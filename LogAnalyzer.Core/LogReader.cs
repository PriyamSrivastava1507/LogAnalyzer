namespace LogAnalyzer.Core;

public record ReadResult(List<LogEntry> Entries, List<string> BadLines);

public static class LogReader
{
    public static ReadResult ReadFile(string path)
    {
        var entries = new List<LogEntry>();
        var badLines = new List<string>();

        foreach (var line in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                entries.Add(LogParser.Parse(line));
            }

            catch (LogParseException ex)
            {
                badLines.Add(ex.Line);
            }
        }

        return new(entries, badLines);
    }
}
