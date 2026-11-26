using System.Security.Cryptography.X509Certificates;

namespace LogAnalyzer.Core;

public static class LogFormatDetector
{
    private const int SampleSize = 20;

    private static bool Parses(ILogParser parser, string line)
    {
        try
        {
            parser.Parse(line);
            return true;
        }
        catch (LogParseException)
        {
            return false;
        }
    }

    public static ILogParser Detect(IEnumerable<string> lines)
    {
        var sample = lines
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .Take(SampleSize)
        .ToList();

        ILogParser[] candidates =
        [
          new JsonLogParser(),
          new AccessLogParser(),
          new PlainTextLogParser()
        ];

        var (bestParser, bestHits) = candidates
            .Select(parser => (Parser: parser, Hits: sample.Count(line => Parses(parser, line))))
            .OrderByDescending(x => x.Hits)
            .First();

        return bestHits > 0 ? bestParser : new PlainTextLogParser();
    }

    public static ILogParser DetectFromFile(string path) => Detect(File.ReadLines(path));

}