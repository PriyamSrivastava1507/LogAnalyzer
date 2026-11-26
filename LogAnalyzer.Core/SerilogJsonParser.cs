using System.Text.Json;

namespace LogAnalyzer.Core;

public class SerilogJsonParser : LogParserBase
{
    private static LogLevel MapLevel(string? text, string line) => text switch
    {
        null => LogLevel.Info,
        "Verbose" or "Debug" => LogLevel.Debug,
        "Information" => LogLevel.Info,
        "Warning" => LogLevel.Warn,
        "Error" => LogLevel.Error,
        "Fatal" => LogLevel.Fatal,
        _ => throw new LogParseException($"Unknown Serilog level: '{text}'.", line)
    };

    public override LogEntry Parse(string line)
    {
        try
        {
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;

            var timestamp = ParseTimeStamp(root.GetProperty("@t").GetString() ?? "", line);
            var level = MapLevel(root.TryGetProperty("@l", out var l) ? l.GetString() : null, line);

            var message = root.TryGetProperty("@m", out var rendered)
                ? rendered.GetString()
                : root.TryGetProperty("@mt", out var template) ? template.GetString() : null;

            var source = root.TryGetProperty("SourceContext", out var context) ? context.GetString() : null;

            var fields = new Dictionary<string, string>();
            foreach (var property in root.EnumerateObject())
            {
                if (property.Name.StartsWith('@') || property.Name == "SourceContext")
                    continue;

                fields[property.Name] = property.Value.ToString();
            }

            if (root.TryGetProperty("@x", out var exception))
                fields["exception"] = exception.GetString() ?? "";

            return new LogEntry(timestamp, level, source ?? "serilog", message ?? "", fields.Count > 0 ? fields : null);
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException or FormatException)
        {
            throw new LogParseException($"Invalid Serilog JSON line: {ex.Message}", line);
        }
    }
}