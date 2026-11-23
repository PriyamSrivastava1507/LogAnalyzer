using System.Text.Json;

namespace LogAnalyzer.Core;

public class JsonLogParser : ILogParser
{
    public LogEntry Parse(string line)
    {
        try
        {
            //similar to a context manager in python
            //automatically free resources when the var is no longer used
            //also JsonDocument reads a line, and then create a tree, like we see in DOM
            using var doc = JsonDocument.Parse(line);
            //here root means the top element
            var root = doc.RootElement;

            var timestamp = root.GetProperty("timestamp").GetDateTime();
            var levelText = root.GetProperty("level").GetString() ?? "";
            var source = root.GetProperty("source").GetString() ?? "";
            var message = root.GetProperty("message").GetString() ?? "";

            if (!Enum.TryParse<LogLevel>(levelText, ignoreCase: true, out var level) || !Enum.IsDefined(level))
                throw new LogParseException($"Unknown log level: '{levelText}'.", line);

            return new LogEntry(timestamp, level, source, message);
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException or FormatException)
        {
            throw new LogParseException($"Invalid JSON log line: {ex.Message}", line);
        }
    }
}