namespace LogAnalyzer.Core;

// traditional way of defining primary constructor
// public class LogParseException : Exception
// {
//     public string Line { get; }

//     public LogParseException(string message, string line) : base(message)
//     {
//         Line = line;
//     }
// }

//modern way of defining primary constructor
public class LogParseException(string message, string line) : Exception(message)
{
    public string Line = line;
}