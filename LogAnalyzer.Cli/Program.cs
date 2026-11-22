using LogAnalyzer.Core;

var newEntry = new LogEntry(DateTime.Now, "ERROR", "AuthService", "LoginFailed");
Console.WriteLine(newEntry);