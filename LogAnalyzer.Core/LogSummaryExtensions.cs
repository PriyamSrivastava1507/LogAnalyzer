namespace LogAnalyzer.Core;

public static class LogSummaryExtensions
{
    public static LogSummary Summarize(this IEnumerable<LogEntry> entries)
    {
        var summary = new LogSummary();
        foreach (var entry in entries)
            summary.Add(entry);

        return summary;
    }
}