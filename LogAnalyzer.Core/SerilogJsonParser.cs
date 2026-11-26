using System.Text.Json;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Core;

public partial class SerilogJsonParser : LogParserBase
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

    [GeneratedRegex(@"\{(?<name>[@$]?\w+)(?:[,:][^}]*)?\}")]
    private static partial Regex TemplateToken();

    private static string RenderTemplate(string template, JsonElement root) =>
        TemplateToken().Replace(template, match =>
        {
            var name = match.Groups["name"].Value.TrimStart('@', '$');
            return root.TryGetProperty(name, out var value) ? value.ToString() : match.Value;
        });

    public override LogEntry Parse(string line)
    {
        try
        {
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;

            var timestamp = ParseTimeStamp(root.GetProperty("@t").GetString() ?? "", line);
            var level = MapLevel(root.TryGetProperty("@l", out var l) ? l.GetString() : null, line);

            string? message;
            if (root.TryGetProperty("@m", out var rendered))
                message = rendered.GetString();
            else if (root.TryGetProperty("@mt", out var template))
                message = RenderTemplate(template.GetString() ?? "", root);
            else
                message = null;

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