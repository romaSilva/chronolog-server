using Chronolog.Api.Domain.Entities;

namespace Chronolog.Api.Contracts;

public record NoteResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string? Content,
    int Year,
    int? Month,
    int? Day,
    DatePrecision DatePrecision,
    long SortableDate,
    IEnumerable<string> Tags
);
