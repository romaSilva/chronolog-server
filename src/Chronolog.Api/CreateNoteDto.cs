namespace Chronolog.Api;

public record CreateNoteDto
{
    public required Guid UserId { get; set; }

    public required string Title { get; set; }

    public string? Content { get; set; }

    public required int Year { get; set; }

    public int? Month { get; set; }

    public int? Day { get; set; }
}