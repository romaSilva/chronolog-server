using Chronolog.Api.Domain.Entities;
using Chronolog.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chronolog.Api.Infrastructure.Repositories;

public class NoteRepository(ChronologDbContext context) : INoteRepository
{
    public async Task AddAsync(Note note, CancellationToken cancellationToken = default)
    {
        context.Notes.Add(note);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Note>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Notes
            .Include(n => n.NoteTags)
            .ThenInclude(nt => nt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<Note?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Notes
            .Include(n => n.NoteTags)
            .ThenInclude(nt => nt.Tag)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var note = await context.Notes.FindAsync([id], cancellationToken);
        if (note is null)
            return false;

        context.Notes.Remove(note);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
