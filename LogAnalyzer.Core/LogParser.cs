namespace LogAnalyzer.Core;

public static class LogParser
{
    public static LogEntry Parse(string line)
    {
        var parts = line.Split(' ', 5);

        if (parts.Length < 5)
            throw new LogParseException("Expected 5 parts: date, time, level, source, message.", line);

        if (!DateTime.TryParse($"{parts[0]} {parts[1]}", out var timestamp))
            throw new LogParseException($"Invalid timestamp: '{parts[0]} {parts[1]}'.", line);

        return new LogEntry(timestamp, parts[2], parts[3], parts[4]);
    }
}