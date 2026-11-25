namespace LogAnalyzer.Core;

public class LogSummary
{
    private readonly Dictionary<LogLevel, int> _byLevel = [];
    private readonly Dictionary<string, int> _bySource = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<int, int> _byHour = [];

    public int Total { get; private set; }
    public IReadOnlyDictionary<LogLevel, int> ByLevel => _byLevel;
    public IReadOnlyDictionary<string, int> BySource => _bySource;
    public IReadOnlyDictionary<int, int> ByHour => _byHour;

    public void Add(LogEntry entry)
    {
        Total++;
        _byLevel[entry.Level] = _byLevel.GetValueOrDefault(entry.Level) + 1;
        _bySource[entry.Source] = _bySource.GetValueOrDefault(entry.Source) + 1;
        _byHour[entry.TimeStamp.Hour] = _byHour.GetValueOrDefault(entry.TimeStamp.Hour) + 1;
    }
}