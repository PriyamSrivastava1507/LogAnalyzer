using System.Globalization;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Core;

public partial class AccessLogParser : LogParserBase
{
    [GeneratedRegex("""^(?<ip>\S+) \S+ (?<user>\S+) \[(?<time>[^\]]+)\] "(?<method>\S+) (?<path>\S+)[^"]*" (?<status>\d{3}) (?<bytes>\S+)(?: "(?<referer>[^"]*)" "(?<agent>[^"]*)")?$""")]
    private static partial Regex AccessLine();

    public override LogEntry Parse(string line)
    {
        var m = AccessLine().Match(line);
        if (!m.Success)
            throw new LogParseException("Not a valid access log line.", line);

        var timestamp = ParseAccessTime(m.Groups["time"].Value, line);
        var status = int.Parse(m.Groups["status"].Value);

        var level = status switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warn,
            _ => LogLevel.Info
        };

        var fields = new Dictionary<string, string>
        {
            ["ip"] = m.Groups["ip"].Value,
            ["method"] = m.Groups["method"].Value,
            ["path"] = m.Groups["path"].Value,
            ["status"] = m.Groups["status"].Value,
            ["bytes"] = m.Groups["bytes"].Value
        };
        if (m.Groups["agent"].Success)
            fields["userAgent"] = m.Groups["agent"].Value;

        var message = $"{m.Groups["method"].Value} {m.Groups["path"].Value} {status}";
        return new LogEntry(timestamp, level, "access", message, fields);
    }

    private static DateTimeOffset ParseAccessTime(string text, string line)
    {
        var parts = text.Split(' ');
        if (parts.Length != 2 || parts[1].Length != 5
            || !DateTimeOffset.TryParseExact(
                $"{parts[0]} {parts[1][..3]}:{parts[1][3..]}",
                "dd/MMM/yyyy:HH:mm:ss zzz",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var timestamp))
        {
            throw new LogParseException($"Invalid access-log timestamp: '{text}'.", line);
        }

        return timestamp;
    }
}