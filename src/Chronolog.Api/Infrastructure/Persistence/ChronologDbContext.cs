using Chronolog.Api.Domain.Entities;
using Chronolog.Api.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Chronolog.Api.Infrastructure.Persistence;

public class ChronologDbContext : DbContext
{
    public DbSet<Note> Notes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<NoteTag> NoteTags { get; set; }

    public ChronologDbContext(DbContextOptions<ChronologDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new NoteConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new NoteTagConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var noteEntries = ChangeTracker
            .Entries<Note>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .ToList();

        foreach (var note in noteEntries)
        {
            note.CalculateFields();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}