using LogAnalyzer.Core;

namespace LogAnalyzer.Cli;

public static class ConsoleHelpers
{
    public static ConsoleColor ColorFor(LogLevel level) => level switch
    {
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Warn => ConsoleColor.Yellow,
        LogLevel.Info => ConsoleColor.Green,
        _ => ConsoleColor.Gray
    };

    public static void PrintEntry(LogEntry entry, IReadOnlySet<string> criticalSources)
    {
        Console.ForegroundColor = ColorFor(entry.Level);
        var marker = entry is { Level: LogLevel.Error } && criticalSources.Contains(entry.Source)
            ? $" [{entry.Source.ToUpperInvariant()} ALERT]"
            : "";
        Console.WriteLine($"  {entry.TimeStamp:HH:mm:ss} {entry.Level} {entry.Source}: {entry.Message}{marker}");
        Console.ResetColor();
    }
}
