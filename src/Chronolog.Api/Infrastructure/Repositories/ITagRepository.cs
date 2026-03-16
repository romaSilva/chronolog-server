using Chronolog.Api.Domain.Entities;

namespace Chronolog.Api.Infrastructure.Repositories;

public interface ITagRepository
{
    Task<Tag?> FindByNormalizedNameAsync(Guid userId, string normalizedName, CancellationToken cancellationToken = default);
    void Add(Tag tag);
}
