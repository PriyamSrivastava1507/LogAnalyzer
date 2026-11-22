using LogAnalyzer.Core;

List<string> lines =
[
    "2026-11-21 10:42:13 ERROR AuthService Login failed",
    "hello",
    "not-a-date 10:42:13 INFO Db Connected"
];

foreach (var line in lines)
{
    try
    {
        var entry = LogParser.Parse(line);
        Console.WriteLine(entry);
    }
    catch (LogParseException ex)
    {
        Console.WriteLine($"Skipped bad line: {ex.Message} -> \"{ex.Line}\"");
    }
}