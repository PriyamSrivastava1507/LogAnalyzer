using LogAnalyzer.Core;

var path = args.Length > 0 ? args[0] : "sample.log";
var result = LogReader.ReadFile(path);

Console.WriteLine($"Parsed {result.Entries.Count} entries, skipped {result.BadLines.Count} bad lines.");

foreach (var entry in result.Entries)
    Console.WriteLine(entry);