using LogAnalyzer.Core;
using Xunit;

namespace LogAnalyzer.Tests;

public class AccessLogParserTests
{
    private readonly AccessLogParser _parser = new();

    [Fact]
    public void Parse_ValidLine_ExtractsFields()
    {
        var entry = _parser.Parse("203.0.113.5 - - [21/Nov/2026:13:00:01 +0000] \"GET /index.html HTTP/1.1\" 200 1024 \"-\" \"Mozilla/5.0\"");

        Assert.Equal(new DateTimeOffset(2026, 11, 21, 13, 0, 1, TimeSpan.Zero), entry.TimeStamp);
        Assert.Equal("GET /index.html 200", entry.Message);
        Assert.Equal("203.0.113.5", entry.Fields!["ip"]);
        Assert.Equal("/index.html", entry.Fields["path"]);
    }

    [Theory]
    [InlineData(200, LogLevel.Info)]
    [InlineData(404, LogLevel.Warn)]
    [InlineData(503, LogLevel.Error)]
    public void Parse_MapsStatusToLevel(int status, LogLevel expected)
    {
        var entry = _parser.Parse($"1.2.3.4 - - [21/Nov/2026:13:00:01 +0000] \"GET /x HTTP/1.1\" {status} 10 \"-\" \"ua\"");

        Assert.Equal(expected, entry.Level);
    }

    [Fact]
    public void Parse_BadLine_ThrowsLogParseException()
    {
        Assert.Throws<LogParseException>(() => _parser.Parse("this is not an access line"));
    }
}