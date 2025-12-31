using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface IKeywordRuleMatcher
{
    string? TryResolve(FileItem file);
}