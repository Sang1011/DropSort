using DropSort.Core.Interfaces;
using DropSort.Core.Models;

namespace Application.Services;

public class KeywordRuleMatcher : IKeywordRuleMatcher
{
    private readonly IKeywordRuleRepository _repo;

    public KeywordRuleMatcher(IKeywordRuleRepository repo)
    {
        _repo = repo;
    }

    public string? TryResolve(FileItem file)
    {
        var rules = _repo.GetEnabledRules()
            .OrderByDescending(r => r.Priority);

        foreach (var rule in rules)
        {
            if (!file.FileName.Contains(
                    rule.Keyword,
                    StringComparison.OrdinalIgnoreCase))
                continue;

            if (rule.Extensions is { Count: > 0 })
            {
                var ext = file.Extension.TrimStart('.').ToLowerInvariant();
                if (!rule.Extensions.Contains(ext))
                    continue;
            }

            return rule.TargetFolder;
        }

        return null;
    }
}
