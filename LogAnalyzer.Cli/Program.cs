using LogAnalyzer.Cli;
using LogAnalyzer.Core;

var criticalSources = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    // Data
    "Db", "Database", "Sql", "Postgres", "MySql", "Mongo", "Redis", "Cache", "Storage", "Disk",

    // Security and identity
    "Auth", "AuthService", "Security", "Firewall", "Identity", "Oauth",

    // Business-critical services
    "Api", "Gateway", "Payment", "Payments", "Billing", "Checkout", "Order",

    // Infrastructure
    "Network", "Dns", "Kernel", "Memory", "Cpu", "Scheduler", "Queue", "Broker", "Kafka",
};

var paths = args.Length > 0 ? args : ["sample.log"];
var results = new Dictionary<string, ReadResult>();

foreach (var path in paths)
{
    try
    {
        results[path] = LogReader.ReadFile(path, LogParserSelector.Select(path));
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
    ConsoleHelpers.PrintEntry(entry, criticalSources);

var start = new DateTimeOffset(2026, 11, 21, 10, 40, 0, TimeSpan.Zero);
var end = new DateTimeOffset(2026, 11, 21, 10, 50, 0, TimeSpan.Zero);

Console.WriteLine();
Console.WriteLine("AuthService errors between 10:40 and 10:50:");
foreach (var entry in allEntries.FromSource("authservice").AtLevel(LogLevel.Error).Between(start, end))
    Console.WriteLine($"  {entry.TimeStamp:HH:mm:ss} {entry.Message}");

Console.WriteLine();
Console.WriteLine("Entries for requestId req-1:");
foreach (var entry in allEntries.WithField("requestId", "req-1"))
    Console.WriteLine($"  {entry.TimeStamp:HH:mm:ss} {entry.Source}: {entry.Message}");

var errorCount = 0;
var badCount = 0;
using (var demoReader = new StreamReader("sample.log"))
{
    foreach (var entry in LogReader.StreamEntries(demoReader, new PlainTextLogParser(), _ => badCount++))
    {
        if (entry.Level == LogLevel.Error)
            errorCount++;
    }
}
Console.WriteLine($"Streamed sample.log: {errorCount} errors, {badCount} bad lines");
