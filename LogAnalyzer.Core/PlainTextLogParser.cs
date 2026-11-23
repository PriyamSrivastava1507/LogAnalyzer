namespace LogAnalyzer.Core;

public class PlainTextLogParser : ILogParser
{
    public LogEntry Parse(string line)
    {
        var parts = line.Split(' ', 5);

        if (parts.Length < 5)
            throw new LogParseException("Expected 5 parts: date, time, level, source, message.", line);

        if (!DateTime.TryParse($"{parts[0]} {parts[1]}", out var timestamp))
            throw new LogParseException($"Invalid timestamp: '{parts[0]} {parts[1]}'.", line);

        if (!Enum.TryParse<LogLevel>(parts[2], ignoreCase: true, out var level) || !Enum.IsDefined(level))
            throw new LogParseException($"Unknown log level: '{parts[2]}'.", line);

        return new LogEntry(timestamp, level, parts[3], parts[4]);
    }
}