using LogAnalyzer.Core;

namespace LogAnalyzer.Cli;

public static class ConsoleHelpers
{
    public static ConsoleColor ColorFor(LogLevel level) => level switch
    {
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Warn => ConsoleColor.Yellow,
        LogLevel.Info => ConsoleColor.Green,
        LogLevel.Fatal => ConsoleColor.Magenta,
        _ => ConsoleColor.Gray
    };

    public static void PrintEntry(LogEntry entry, IReadOnlySet<string> criticalSources)
    {
        var lines = entry.Message.Split('\n');

        var shown = lines.Length > 1 ? $"{lines[0]} (+{lines.Length - 1} more lines)" : lines[0];

        Console.ForegroundColor = ColorFor(entry.Level);
        var marker = entry.Level >= LogLevel.Error && criticalSources.Contains(entry.Source)
            ? $" [{entry.Source.ToUpperInvariant()} ALERT]"
            : "";
        Console.WriteLine($"  {entry.TimeStamp:HH:mm:ss} {entry.Level} {entry.Source}: {shown}{marker}");
        Console.ResetColor();
    }
}
