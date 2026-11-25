using LogAnalyzer.Core;
using Xunit;

namespace LogAnalyzer.Tests;

public class JsonLogParserTests
{
    private readonly JsonLogParser _parser = new();

    [Fact]
    public void Parse_ValidLine_CapturesExtraFields()
    {
        var entry = _parser.Parse("""{"timestamp":"2026-11-21T12:00:00","level":"error","source":"Api","message":"Request failed","requestId":"req-1","status":500}""");

        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.NotNull(entry.Fields);
        Assert.Equal("req-1", entry.Fields["requestId"]);
        Assert.Equal("500", entry.Fields["status"]);
    }

    [Theory]
    [InlineData("not json at all")]
    [InlineData("""{"timestamp":"2026-11-21T12:00:09","level":"warn","source":"Cache"}""")]
    [InlineData("""{"timestamp":"2026-11-21T12:00:09","level":"bogus","source":"Cache","message":"x"}""")]
    public void Parse_BadLine_ThrowsLogParseException(string line)
    {
        Assert.Throws<LogParseException>(() => _parser.Parse(line));
    }
}