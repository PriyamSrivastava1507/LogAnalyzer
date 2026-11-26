namespace LogAnalyzer.Core;

public static class LogParserSelector
{
    public static ILogParser Select(string path)
    {
        if (Path.GetFileName(path).Contains("access", StringComparison.OrdinalIgnoreCase))
            return new AccessLogParser();

        return Path.GetExtension(path).Equals(".jsonl", StringComparison.OrdinalIgnoreCase)
            ? new JsonLogParser()
            : new PlainTextLogParser();
    }
}