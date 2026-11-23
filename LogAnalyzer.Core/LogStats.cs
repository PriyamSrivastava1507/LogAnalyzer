namespace LogAnalyzer.Core;

public static class LogStats
{
    public static IEnumerable<(TKey Key, int Count)> CountBySelector<TKey>(this IEnumerable<LogEntry> entries, Func<LogEntry, TKey> keySelector) =>
        entries
            .GroupBy(keySelector)
            .Select(g => (Level: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count);
}