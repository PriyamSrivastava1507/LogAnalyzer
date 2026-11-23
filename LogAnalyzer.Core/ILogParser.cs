namespace LogAnalyzer.Core;

public interface ILogParser
{
    LogEntry Parse(string line);
}

