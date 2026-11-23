namespace LogAnalyzer.Core;

public abstract class LogParserBase : ILogParser
{
    public abstract LogEntry Parse(string line);

    protected static LogLevel ParseLevel(string text, string line)
    {
        if (!Enum.TryParse<LogLevel>(text, ignoreCase: true, out var level) || !Enum.IsDefined(level))
            throw new LogParseException($"Unknown log level: '{text}'.", line);

        return level;
    }
}