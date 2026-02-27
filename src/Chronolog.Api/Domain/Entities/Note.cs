namespace Chronolog.Api.Domain.Entities;

public class Note
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }

    public required string Title { get; set; }
    public string? Content { get; set; }

    public required int Year { get; set; }
    public int? Month { get; set; }
    public int? Day { get; set; }

    public DatePrecision DatePrecision { get; private set; }
    public long SortableDate { get; private set; }

    public string? Metadata { get; set; }

    public ICollection<NoteTag> NoteTags { get; set; } = [];

    /// <summary>
    /// Validates and calculates DatePrecision and SortableDate based on Year, Month, and Day.
    /// Must be called before persisting to ensure data consistency.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when Month/Day combination is invalid.</exception>
    public void CalculateFields()
    {
        ValidateDatePrecision();
        DatePrecision = CalculateDatePrecision();
        SortableDate = CalculateSortableDate();
    }

    private void ValidateDatePrecision()
    {
        if (Month is null && Day is not null)
        {
            throw new InvalidOperationException("Day cannot be set without Month.");
        }

        if (Month is not null && (Month < 1 || Month > 12))
        {
            throw new InvalidOperationException($"Month must be between 1 and 12, got {Month}.");
        }

        if (Day is not null && (Day < 1 || Day > 31))
        {
            throw new InvalidOperationException($"Day must be between 1 and 31, got {Day}.");
        }
    }

    private DatePrecision CalculateDatePrecision()
    {
        return (Month, Day) switch
        {
            (null, null) => DatePrecision.Year,
            (not null, null) => DatePrecision.Month,
            (not null, not null) => DatePrecision.Day,
            _ => throw new InvalidOperationException("Invalid combination of Month and Day.")
        };
    }

    private long CalculateSortableDate(int offset = 1_000_000)
        => ((long)(Year + offset) * 10000)
           + ((Month ?? 0) * 100)
           + (Day ?? 0);
}

public enum DatePrecision
{
    Year,
    Month,
    Day
}