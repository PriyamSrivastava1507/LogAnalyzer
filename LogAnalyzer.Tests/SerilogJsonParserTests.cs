using LogAnalyzer.Core;
using Xunit;

namespace LogAnalyzer.Tests;

public class SerilogJsonParserTests
{
    private readonly SerilogJsonParser _parser = new();

    [Fact]
    public void Parse_ValidLine_ExtractsEverything()
    {
        var entry = _parser.Parse("""{"@t":"2026-11-21T14:00:09.5000000Z","@mt":"Payment failed for {OrderId}","@m":"Payment failed for 7","@l":"Error","OrderId":7,"@x":"System.TimeoutException: timed out","SourceContext":"MyApp.PaymentService"}""");

        Assert.Equal(new DateTimeOffset(2026, 11, 21, 14, 0, 9, 500, TimeSpan.Zero), entry.TimeStamp);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("MyApp.PaymentService", entry.Source);
        Assert.Equal("Payment failed for 7", entry.Message);
        Assert.Equal("7", entry.Fields!["OrderId"]);
        Assert.Contains("TimeoutException", entry.Fields["exception"]);
    }

    [Fact]
    public void Parse_MissingLevel_MeansInfo()
    {
        var entry = _parser.Parse("""{"@t":"2026-11-21T14:00:00Z","@mt":"hello"}""");

        Assert.Equal(LogLevel.Info, entry.Level);
    }

    [Theory]
    [InlineData("Verbose", LogLevel.Debug)]
    [InlineData("Information", LogLevel.Info)]
    [InlineData("Warning", LogLevel.Warn)]
    [InlineData("Fatal", LogLevel.Fatal)]
    public void Parse_MapsSerilogLevels(string levelText, LogLevel expected)
    {
        var entry = _parser.Parse($$"""{"@t":"2026-11-21T14:00:00Z","@l":"{{levelText}}","@mt":"x"}""");

        Assert.Equal(expected, entry.Level);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("""{"timestamp":"2026-11-21T12:00:00","level":"info","source":"Api","message":"ok"}""")]
    public void Parse_BadLine_ThrowsLogParseException(string line)
    {
        Assert.Throws<LogParseException>(() => _parser.Parse(line));
    }

    [Fact]
    public void Detector_PicksSerilogParser()
    {
        var parser = LogFormatDetector.Detect(["""{"@t":"2026-11-21T14:00:00Z","@mt":"hello"}"""]);

        Assert.IsType<SerilogJsonParser>(parser);
    }

    [Fact]
    public void Parse_RendersTemplateWhenNoRenderedMessage()
    {
        var entry = _parser.Parse("""{"@t":"2026-11-21T14:00:00Z","@mt":"User {UserId} paid {Amount:N2}","UserId":42,"Amount":12.5}""");

        Assert.Equal("User 42 paid 12.5", entry.Message);
    }

    [Fact]
    public void Parse_KeepsTokenWhenPropertyIsMissing()
    {
        var entry = _parser.Parse("""{"@t":"2026-11-21T14:00:00Z","@mt":"Hello {Nobody}"}""");

        Assert.Equal("Hello {Nobody}", entry.Message);
    }

    [Fact]
    public void Parse_PrefersRenderedMessage()
    {
        var entry = _parser.Parse("""{"@t":"2026-11-21T14:00:00Z","@mt":"User {UserId}","@m":"User 7 (rendered)","UserId":42}""");

        Assert.Equal("User 7 (rendered)", entry.Message);
    }
}