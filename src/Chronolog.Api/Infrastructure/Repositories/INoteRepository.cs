using Chronolog.Api.Domain.Entities;

namespace Chronolog.Api.Infrastructure.Repositories;

public interface INoteRepository
{
    Task AddAsync(Note note, CancellationToken cancellationToken = default);
    Task<IEnumerable<Note>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Note?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
