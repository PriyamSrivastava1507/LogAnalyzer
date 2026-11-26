namespace LogAnalyzer.Core;

public static class LogParserSelector
{
    public static ILogParser Select(string path)
    {
        return LogFormatDetector.DetectFromFile(path);
    }
}