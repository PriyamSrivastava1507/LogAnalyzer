namespace LogAnalyzer.Core;

public record ReadResult(List<LogEntry> Entries, List<string> BadLines);

public static class LogReader
{
    public static ReadResult ReadFile(string path, ILogParser parser)
    {
        using var reader = new StreamReader(path);
        return Read(reader, parser);
    }
    public static ReadResult Read(TextReader reader, ILogParser parser)
    {
        var entries = new List<LogEntry>();
        var badLines = new List<string>();

        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                entries.Add(parser.Parse(line));
            }

            catch (LogParseException ex)
            {
                badLines.Add(ex.Line);
            }
        }

        return new(entries, badLines);
    }
}
