using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface IKeywordRuleRepository
{
    IReadOnlyList<KeywordRule> GetEnabledRules();
}