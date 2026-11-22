using LogAnalyzer.Core;

var paths = args.Length > 0 ? args : ["sample.log"];
var results = new Dictionary<string, ReadResult>();

foreach (var path in paths)
{
    try
    {
        results[path] = LogReader.ReadFile(path);
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

var allEntries = results.Values.SelectMany(r => r.Entries).ToList();

Console.WriteLine();
Console.WriteLine("Entries by level (all files):");
foreach (var (level, count) in LogStats.CountByLevel(allEntries))
    Console.WriteLine($"  {level}: {count}");

Console.WriteLine();
Console.WriteLine("Entries by Source (all files):");
foreach (var (source, count) in LogStats.CountBySource(allEntries))
    Console.WriteLine($"  {source}: {count}");

