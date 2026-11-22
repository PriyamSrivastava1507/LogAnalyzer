namespace LogAnalyzer.Core;

public record LogEntry(DateTime TimeStamp, string Level, string Source, string Message);
