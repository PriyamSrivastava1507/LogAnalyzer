using LogAnalyzer.Core;
using Xunit;

namespace LogAnalyzer.Tests;

public class LogSummaryTests
{
    private static LogEntry Entry(int hour, LogLevel level, string source) =>
        new(new DateTimeOffset(2026, 11, 21, hour, 0, 0, TimeSpan.Zero), level, source, "msg");

    [Fact]
    public void Summarize_CountsByLevelSourceAndHour()
    {
        var entries = new[]
        {
            Entry(10, LogLevel.Error, "Api"),
            Entry(10, LogLevel.Error, "api"),
            Entry(11, LogLevel.Info, "Db")
        };

        var summary = entries.Summarize();

        Assert.Equal(3, summary.Total);
        Assert.Equal(2, summary.ByLevel[LogLevel.Error]);
        Assert.Equal(2, summary.BySource["Api"]);
        Assert.Equal(2, summary.ByHour[10]);
    }

    [Fact]
    public void Summarize_EnumeratesTheSourceOnlyOnce()
    {
        var passes = 0;

        IEnumerable<LogEntry> Source()
        {
            passes++;
            yield return Entry(10, LogLevel.Info, "Api");
            yield return Entry(11, LogLevel.Warn, "Db");
        }

        Source().Summarize();

        Assert.Equal(1, passes);
    }
}