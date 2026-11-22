namespace LogAnalyzer.Core;

public static class LogStats
{
    public static IEnumerable<(string Level, int Count)> CountByLevel(IEnumerable<LogEntry> entries) =>
        entries
            .GroupBy(e => e.Level)
            .Select(g => (Level: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count);

    public static IEnumerable<(string Source, int Count)> CountBySource(IEnumerable<LogEntry> entries) =>
    entries
        .GroupBy(e => e.Source)
        .Select(g => (Source: g.Key, Count: g.Count()))
        .OrderByDescending(x => x.Count);
}