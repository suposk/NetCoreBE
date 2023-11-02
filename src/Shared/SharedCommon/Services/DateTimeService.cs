namespace SharedCommon.Services;

public interface IDateTimeService
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow { get; } = DateTime.UtcNow;
    public DateTime Now { get; } = DateTime.Now;
}

public class TestDateTimeService : IDateTimeService
{
    static readonly DateTime _utc = new(2023, 5, 16, 13, 25, 55);
    public DateTime UtcNow { get; } = _utc;
    public DateTime Now { get; } = _utc.AddHours(1);
}
