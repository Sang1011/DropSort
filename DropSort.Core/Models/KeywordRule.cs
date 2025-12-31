namespace DropSort.Core.Models;

public class KeywordRule
{
    public int Id { get; init; }
    public string Keyword { get; init; } = string.Empty;
    public IReadOnlyList<string>? Extensions { get; init; }
    public string TargetFolder { get; init; } = string.Empty;
    public int Priority { get; init; }
    public bool Enabled { get; init; }
}