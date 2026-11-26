using LogAnalyzer.Core;
using Xunit;

namespace LogAnalyzer.Tests;

public class LogFormatDetectorTests
{
    [Fact]
    public void Detect_JsonLines_ReturnsJsonParser()
    {
        var parser = LogFormatDetector.Detect(
            ["""{"timestamp":"2026-11-21T12:00:00","level":"info","source":"Api","message":"ok"}"""]);

        Assert.IsType<JsonLogParser>(parser);
    }

    [Fact]
    public void Detect_AccessLines_ReturnsAccessParser()
    {
        var parser = LogFormatDetector.Detect(
            ["203.0.113.5 - - [21/Nov/2026:13:00:01 +0000] \"GET /x HTTP/1.1\" 200 10 \"-\" \"ua\""]);

        Assert.IsType<AccessLogParser>(parser);
    }

    [Fact]
    public void Detect_PlainText_ReturnsPlainTextParser()
    {
        var parser = LogFormatDetector.Detect(["2026-11-21 10:42:13 ERROR Api Login failed"]);

        Assert.IsType<PlainTextLogParser>(parser);
    }

    [Fact]
    public void Detect_UnrecognizedOrEmpty_FallsBackToPlainText()
    {
        Assert.IsType<PlainTextLogParser>(LogFormatDetector.Detect(["garbage", "more garbage"]));
        Assert.IsType<PlainTextLogParser>(LogFormatDetector.Detect([]));
    }
}