namespace DropSort.Core.Models;

public class LogEntry
{
    public string Level { get; set; } = "INFO";
    public string Message { get; set; } = string.Empty;

    public string? FileName { get; set; }
    public string? SourcePath { get; set; }
    public string? TargetPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}