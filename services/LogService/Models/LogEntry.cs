namespace LogService.Models;

public class LogEntry
{
    public int Id {get; set;}
    public string ServiceName {get; set;}  = String.Empty;
    public string Level {get; set;} = String.Empty;
    public string Message {get; set;} = String.Empty;
    public DateTime Timestamp {get; set;} = DateTime.UtcNow;
}