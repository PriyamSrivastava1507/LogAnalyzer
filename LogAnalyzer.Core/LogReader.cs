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
        var badLines = new List<string>();
        var entries = StreamEntries(reader, parser, badLines.Add).ToList();
        return new ReadResult(entries, badLines);
    }

    //Here Action means a function that does not return anything, so it can take any function that doesnot return anything as arguments
    //If your function returns something then use Func, Func<Input1, Input2, ....,  Output>
    //to run them, just use .Invoke() on them
    //the ? after the action name means if it is null skip it
    public static IEnumerable<LogEntry> StreamEntries(
        TextReader reader, ILogParser parser, Action<string>? onBadLine = null
    )
    {
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            LogEntry entry;
            try
            {
                entry = parser.Parse(line);
            }
            catch (LogParseException ex)
            {
                onBadLine?.Invoke(ex.Line);
                continue;
            }

            yield return entry;
        }
    }
}
