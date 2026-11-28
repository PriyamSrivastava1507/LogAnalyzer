namespace LogAnalyzer.Core;

public interface ILogParser
{
    LogEntry Parse(string line);
    bool IsRecordStart(string line);
}

