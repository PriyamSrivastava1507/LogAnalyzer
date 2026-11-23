namespace LogAnalyzer.Core;

public static class LogParserSelector
{
    public static ILogParser Select(string path) =>
        Path.GetExtension(path).Equals(".jsonl", StringComparison.OrdinalIgnoreCase)
            ? new JsonLogParser()
            : new PlainTextLogParser();
}