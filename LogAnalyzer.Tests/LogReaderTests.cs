using LogAnalyzer.Core;
using Xunit;

namespace LogAnalyzer.Tests;

public class LogReaderTests
{
    [Fact]
    public void StreamEntries_SkipsBadLinesAndReportsThem()
    {
        var text = "2026-11-21 10:00:00 INFO Api ok\n2026-11-21 10:00:05 BOGUS Api nope\n\n2026-11-21 10:00:01 ERROR Db fail";
        var bad = new List<string>();

        var entries = LogReader.StreamEntries(new StringReader(text), new PlainTextLogParser(), bad.Add).ToList();

        Assert.Equal(2, entries.Count);
        Assert.Single(bad);
        Assert.Equal("2026-11-21 10:00:05 BOGUS Api nope", bad[0]);
    }

    [Fact]
    public void StreamEntries_IsLazy_ParsesOnlyWhatIsConsumed()
    {
        var text = "2026-11-21 10:00:00 INFO Api one\n2026-11-21 10:00:01 INFO Api two\ngarbage";
        var bad = new List<string>();

        var first = LogReader.StreamEntries(new StringReader(text), new PlainTextLogParser(), bad.Add).First();

        Assert.Equal("one", first.Message);
        Assert.Empty(bad);
    }

    [Fact]
    public void StreamEntries_JoinsStackTraceLinesIntoPreviousEntry()
    {
        var text = "2026-11-21 10:00:00 ERROR Api Unhandled exception\nSystem.InvalidOperationException: Boom\n   at MyApp.Run()\n2026-11-21 10:00:01 INFO Api next";

        var entries = LogReader.StreamEntries(new StringReader(text), new PlainTextLogParser()).ToList();

        Assert.Equal(2, entries.Count);
        Assert.Equal("Unhandled exception\nSystem.InvalidOperationException: Boom\n   at MyApp.Run()", entries[0].Message);
        Assert.Equal("next", entries[1].Message);
    }
}