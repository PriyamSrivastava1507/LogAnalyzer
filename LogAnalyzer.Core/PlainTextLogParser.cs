using System.Text.RegularExpressions;

namespace LogAnalyzer.Core;

public partial class PlainTextLogParser : LogParserBase
{
    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}")]
    private static partial Regex RecordStart();

    public override bool IsRecordStart(string line) => RecordStart().IsMatch(line);

    public override LogEntry Parse(string line)
    {
        var parts = line.Split(' ', 5);

        if (parts.Length < 5)
            throw new LogParseException("Expected 5 parts: date, time, level, source, message.", line);

        var timestamp = ParseTimeStamp($"{parts[0]} {parts[1]}", line);

        var level = ParseLevel(parts[2], line);

        return new LogEntry(timestamp, level, parts[3], parts[4]);
    }
}