namespace Chronolog.Api.Contracts;

public record UpdateNoteRequest
{
    public required string Title { get; set; }

    public string? Content { get; set; }

    public required int Year { get; set; }

    public int? Month { get; set; }

    public int? Day { get; set; }

    public IEnumerable<string>? Tags { get; init; } = [];
}
