using Chronolog.Api.Contracts;
using Chronolog.Api.Domain.Entities;
using Chronolog.Api.Infrastructure.Repositories;

namespace Chronolog.Api.Application.Handlers;

public class CreateNoteHandler(INoteRepository noteRepository, ITagRepository tagRepository)
{
    public async Task<NoteResponse> HandleAsync(CreateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var note = new Note
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Content = request.Content,
            Year = request.Year,
            Month = request.Month,
            Day = request.Day
        };

        if (request.Tags is not null)
        {
            // In-request deduplication: track by normalizedName -> Tag
            var resolvedTags = new Dictionary<string, Tag>(StringComparer.Ordinal);

            foreach (var tagName in request.Tags)
            {
                var normalizedName = tagName.Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(normalizedName))
                    continue;

                if (resolvedTags.TryGetValue(normalizedName, out var existing))
                {
                    note.NoteTags.Add(new NoteTag { NoteId = note.Id, TagId = existing.Id, Note = note, Tag = existing });
                    continue;
                }

                var tag = await tagRepository.FindByNormalizedNameAsync(request.UserId, normalizedName, cancellationToken);

                if (tag is null)
                {
                    tag = new Tag
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        Name = tagName.Trim(),
                        NormalizedName = normalizedName
                    };
                    tagRepository.Add(tag);
                }

                resolvedTags[normalizedName] = tag;
                note.NoteTags.Add(new NoteTag { NoteId = note.Id, TagId = tag.Id, Note = note, Tag = tag });
            }
        }

        await noteRepository.AddAsync(note, cancellationToken);

        return new NoteResponse(
            note.Id,
            note.UserId,
            note.Title,
            note.Content,
            note.Year,
            note.Month,
            note.Day,
            note.DatePrecision,
            note.SortableDate,
            note.NoteTags.Select(nt => nt.Tag.NormalizedName)
        );
    }
}
