namespace LogAnalyzer.Core;

public static class LogEntryExtensions
{
    public static IEnumerable<LogEntry> AtLevel(this IEnumerable<LogEntry> entries, LogLevel minLevel) =>
        entries.Where(e => e.Level >= minLevel);

    public static IEnumerable<LogEntry> Between(this IEnumerable<LogEntry> entries, DateTimeOffset start, DateTimeOffset end) =>
        entries.Where(e => e.TimeStamp >= start && e.TimeStamp <= end);

    public static IEnumerable<LogEntry> FromSource(this IEnumerable<LogEntry> entries, string source) =>
        entries.Where(e => e.Source.Equals(source, StringComparison.OrdinalIgnoreCase));
}