using Chronolog.Api.Contracts;
using Chronolog.Api.Domain.Entities;
using Chronolog.Api.Infrastructure.Repositories;

namespace Chronolog.Api.Application.Handlers;

public class NoteHandler(INoteRepository noteRepository, ITagRepository tagRepository)
{
    public async Task<NoteResponse> HandleAsync(CreateNoteRequest request, CancellationToken ct = default)
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

        await ResolveTagsAsync(note, request.Tags, ct);
        await noteRepository.AddAsync(note, ct);

        return ToResponse(note);
    }

    public async Task<NoteResponse?> HandleAsync(Guid id, UpdateNoteRequest request, CancellationToken ct = default)
    {
        var note = await noteRepository.GetByIdAsync(id, ct);
        if (note is null)
            return null;

        note.Title = request.Title;
        note.Content = request.Content;
        note.Year = request.Year;
        note.Month = request.Month;
        note.Day = request.Day;

        note.NoteTags.Clear();
        await ResolveTagsAsync(note, request.Tags, ct);
        await noteRepository.UpdateAsync(note, ct);

        return ToResponse(note);
    }

    private async Task ResolveTagsAsync(Note note, IEnumerable<string>? tags, CancellationToken ct)
    {
        if (tags is null)
            return;

        var resolved = new Dictionary<string, Tag>(StringComparer.Ordinal);

        foreach (var tagName in tags)
        {
            var normalized = tagName.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(normalized))
                continue;

            if (resolved.TryGetValue(normalized, out var existing))
            {
                note.NoteTags.Add(new NoteTag { NoteId = note.Id, TagId = existing.Id, Note = note, Tag = existing });
                continue;
            }

            var tag = await tagRepository.FindByNormalizedNameAsync(note.UserId, normalized, ct)
                      ?? CreateTag(note.UserId, tagName.Trim(), normalized);

            resolved[normalized] = tag;
            note.NoteTags.Add(new NoteTag { NoteId = note.Id, TagId = tag.Id, Note = note, Tag = tag });
        }
    }

    private Tag CreateTag(Guid userId, string name, string normalizedName)
    {
        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            NormalizedName = normalizedName
        };
        tagRepository.Add(tag);
        return tag;
    }

    private static NoteResponse ToResponse(Note note) => new(
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
