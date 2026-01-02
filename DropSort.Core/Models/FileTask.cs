using DropSort.Core.Enums;

namespace DropSort.Core.Models;

public class FileTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FullPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public string SourceDrive { get; set; } = string.Empty;

    // Resolve result
    public FileCategory Category { get; set; }
    public string TargetPath { get; set; } = string.Empty;

    // State
    public FileTaskStatus Status { get; set; } = FileTaskStatus.Pending;

    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}