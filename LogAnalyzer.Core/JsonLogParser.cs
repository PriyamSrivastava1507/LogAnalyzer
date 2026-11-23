using System.Text.Json;

namespace LogAnalyzer.Core;

public class JsonLogParser : LogParserBase
{
    public override LogEntry Parse(string line)
    {
        try
        {
            //similar to a context manager in python
            //automatically free resources when the var is no longer used
            //also JsonDocument reads a line, and then create a tree, like we see in DOM
            using var doc = JsonDocument.Parse(line);
            //here root means the top element
            var root = doc.RootElement;

            var timestamp = ParseTimeStamp(root.GetProperty("timestamp").GetString() ?? "", line);
            var levelText = root.GetProperty("level").GetString() ?? "";
            var source = root.GetProperty("source").GetString() ?? "";
            var message = root.GetProperty("message").GetString() ?? "";

            var level = ParseLevel(levelText, line);

            return new LogEntry(timestamp, level, source, message);
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException or FormatException)
        {
            throw new LogParseException($"Invalid JSON log line: {ex.Message}", line);
        }
    }
}