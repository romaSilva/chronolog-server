using Chronolog.Api.Domain.Entities;
using Chronolog.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chronolog.Api.Infrastructure.Repositories;

public class TagRepository(ChronologDbContext context) : ITagRepository
{
    public Task<Tag?> FindByNormalizedNameAsync(Guid userId, string normalizedName, CancellationToken cancellationToken = default)
        => context.Tags
            .FirstOrDefaultAsync(t => t.UserId == userId && t.NormalizedName == normalizedName, cancellationToken);

    public void Add(Tag tag)
        => context.Tags.Add(tag);
}
