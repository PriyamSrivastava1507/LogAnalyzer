namespace LogAnalyzer.Core;

public record LogEntry(
    DateTimeOffset TimeStamp,
    LogLevel Level,
    string Source,
    string Message,
    IReadOnlyDictionary<string, string>? Fields = null
);
