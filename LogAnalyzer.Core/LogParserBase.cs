using System.Globalization;

namespace LogAnalyzer.Core;

//We have created a abstract class which will be inherited by JsonParser and PlainTextParser instead of directly inheriting ILogParser.
//This is done because both had the common code to parse Enum, so instead of repeating that code again and again
//We put that code in a abstract class
//Abstract class -> for code reuse, a base class which has all the common code and then also forces to implement the function Parse
//Interface -> blueprint for class, the classes implementing me must have these fields or functions
public abstract class LogParserBase : ILogParser
{
    public abstract LogEntry Parse(string line);

    public virtual bool IsRecordStart(string line) => true;

    protected static LogLevel ParseLevel(string text, string line)
    {
        if (!Enum.TryParse<LogLevel>(text, ignoreCase: true, out var level) || !Enum.IsDefined(level))
            throw new LogParseException($"Unknown log level: '{text}'.", line);

        return level;
    }

    protected static DateTimeOffset ParseTimeStamp(string text, string line)
    {
        if (!DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var timestamp))
            throw new LogParseException($"Invalid timestamp: '{text}'.", line);

        return timestamp;
    }
}