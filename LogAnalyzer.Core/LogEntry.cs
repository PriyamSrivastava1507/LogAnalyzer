namespace LogAnalyzer.Core;

public record LogEntry(DateTime TimeStamp, LogLevel Level, string Source, string Message);
