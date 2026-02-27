using Chronolog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolog.Api.Infrastructure.Persistence.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .IsRequired();

        builder.Property(n => n.UserId)
            .IsRequired();

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.Content)
            .HasMaxLength(10000);

        builder.Property(n => n.Year)
            .IsRequired();

        builder.Property(n => n.Month)
            .IsRequired(false);

        builder.Property(n => n.Day)
            .IsRequired(false);

        builder.Property(n => n.DatePrecision)
            .IsRequired();

        builder.Property(n => n.SortableDate)
            .IsRequired();

        builder.Property(n => n.Metadata)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.HasMany(n => n.NoteTags)
            .WithOne(nt => nt.Note)
            .HasForeignKey(nt => nt.NoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Notes");
    }
}
