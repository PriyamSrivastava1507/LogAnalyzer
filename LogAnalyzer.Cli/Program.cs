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
        Console.WriteLine($"File nor found: {path}");
    }
}

foreach (var (path, result) in results)
{
    Console.WriteLine($"{path}: {result.Entries.Count} entries, {result.BadLines.Count} bad lines");
}
