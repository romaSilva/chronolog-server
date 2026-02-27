namespace Chronolog.Api.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }

    public required string Name { get; set; }
    public required string NormalizedName { get; set; }

    public ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();
}