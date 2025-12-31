using DropSort.Core.Enums;

namespace DropSort.Core.Models;

public class FileItem
{
    public string FullPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; init; } = string.Empty;
    public long SizeInBytes { get; init; }
    public string SourceDrive { get; init; } = string.Empty;
    public FileCategory Category { get; set; }
}