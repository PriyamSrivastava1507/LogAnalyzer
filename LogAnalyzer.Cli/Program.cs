using LogAnalyzer.Core;

ILogParser parser = new PlainTextLogParser();

var paths = args.Length > 0 ? args : ["sample.log"];
var results = new Dictionary<string, ReadResult>();

foreach (var path in paths)
{
    try
    {
        results[path] = LogReader.ReadFile(path, parser);
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine($"File not found: {path}");
    }
}

foreach (var (path, result) in results)
{
    Console.WriteLine($"{path}: {result.Entries.Count} entries, {result.BadLines.Count} bad lines");
}

var allEntries = results.AllEntries().ToList();

Console.WriteLine();
Console.WriteLine("Entries by level (all files):");
foreach (var (level, count) in allEntries.CountBySelector(e => e.Level))
    Console.WriteLine($"  {level}: {count}");

var singleEntry = results.EntriesFrom(["sample.log"]);

Console.WriteLine();
Console.WriteLine("Entries from single file (sample.log)");
foreach (var (level, count) in singleEntry.CountBySelector(e => e.Level))
    Console.WriteLine($" {level}: {count}");

Console.WriteLine();
Console.WriteLine("Entries by Source (all files):");
foreach (var (source, count) in allEntries.CountBySelector(e => e.Source))
    Console.WriteLine($"  {source}: {count}");

Console.WriteLine();
Console.WriteLine("Entries by hour (all files):");
foreach (var (hour, count) in allEntries.CountBySelector(e => e.TimeStamp.Hour))
    Console.WriteLine($"  {hour}: {count}");

Console.WriteLine();
Console.WriteLine("Warnings and above (all files):");
foreach (var entry in allEntries.AtLevel(LogLevel.Warn))
    Console.WriteLine($"  {entry.TimeStamp:HH:mm:ss} {entry.Level} {entry.Source}: {entry.Message}");

var start = new DateTime(2026, 11, 21, 10, 40, 0);
var end = new DateTime(2026, 11, 21, 10, 50, 0);

Console.WriteLine();
Console.WriteLine("AuthService errors between 10:40 and 10:50:");
foreach (var entry in allEntries.FromSource("authservice").AtLevel(LogLevel.Error).Between(start, end))
    Console.WriteLine($"  {entry.TimeStamp:HH:mm:ss} {entry.Message}");