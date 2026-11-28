using LogAnalyzer.Core;
using Xunit;

namespace LogAnalyzer.Tests;

public class PlainTextLogParserTests
{
    private readonly PlainTextLogParser _parser = new();

    [Fact]
    public void Parse_ValidLine_ReturnsEntry()
    {
        var entry = _parser.Parse("2026-11-21 10:42:13 ERROR AuthService Login failed");

        Assert.Equal(new DateTimeOffset(2026, 11, 21, 10, 42, 13, TimeSpan.Zero), entry.TimeStamp);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("AuthService", entry.Source);
        Assert.Equal("Login failed", entry.Message);
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("not-a-date 10:42:13 INFO Db Connected")]
    [InlineData("2026-11-21 10:42:13 BOGUS Api Something odd")]
    public void Parse_BadLine_ThrowsLogParseException(string line)
    {
        Assert.Throws<LogParseException>(() => _parser.Parse(line));
    }

    [Theory]
    [InlineData("ERROR", LogLevel.Error)]
    [InlineData("error", LogLevel.Error)]
    [InlineData("Warn", LogLevel.Warn)]
    [InlineData("FATAL", LogLevel.Fatal)]
    public void Parse_LevelIsCaseInsensitive(string text, LogLevel expected)
    {
        var entry = _parser.Parse($"2026-11-21 10:42:13 {text} Api msg");

        Assert.Equal(expected, entry.Level);
    }

    [Theory]
    [InlineData("2026-11-21 10:42:13 ERROR Api x", true)]
    [InlineData("   at MyApp.Run()", false)]
    [InlineData("System.InvalidOperationException: Boom", false)]
    public void IsRecordStart_RecognizesTimestampedLines(string line, bool expected)
    {
        Assert.Equal(expected, _parser.IsRecordStart(line));
    }
}