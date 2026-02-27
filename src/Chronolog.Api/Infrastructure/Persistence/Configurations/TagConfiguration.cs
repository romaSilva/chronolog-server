using Chronolog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolog.Api.Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.NormalizedName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => new { t.UserId, t.NormalizedName })
            .IsUnique()
            .HasDatabaseName("IX_Tags_UserId_NormalizedName_Unique");

        builder.HasMany(t => t.NoteTags)
            .WithOne(nt => nt.Tag)
            .HasForeignKey(nt => nt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Tags");
    }
}
