namespace LogAnalyzer.Core;

public static class LogSelectionExtensions
{
    public static IEnumerable<LogEntry> AllEntries(this IReadOnlyDictionary<string, ReadResult> results) =>
        results.Values.SelectMany(r => r.Entries);

    public static IEnumerable<LogEntry> EntriesFrom(this IReadOnlyDictionary<string, ReadResult> results, IEnumerable<string> paths) =>
        paths.Where(results.ContainsKey).SelectMany(p => results[p].Entries);
}