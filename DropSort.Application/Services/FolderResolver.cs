using DropSort.Core.Enums;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;

namespace Application.Services;

public class FolderResolver : IFolderResolver
{
    private readonly ISettingRepository _settings;
    private readonly IFileClassifier _classifier;
    private readonly IKeywordRuleMatcher _keywordRuleMatcher;

    public FolderResolver(
        ISettingRepository settings,
        IFileClassifier classifier,
        IKeywordRuleMatcher keywordRuleMatcher)
    {
        _settings = settings;
        _classifier = classifier;
        _keywordRuleMatcher = keywordRuleMatcher;
    }

    public string ResolveTargetPath(FileItem file)
    {
        var root = _settings.Get("download_root");
        if (string.IsNullOrWhiteSpace(root))
            throw new InvalidOperationException("download_root is not configured");

        // ===== 1. KEYWORD OVERRIDE (PRIORITY CAO NHẤT) =====
        var keywordFolder = _keywordRuleMatcher.TryResolve(file);
        if (keywordFolder != null)
        {
            return Path.Combine(root, keywordFolder, file.FileName);
        }

        // ===== 2. CATEGORY =====
        var category = _classifier.Classify(file.Extension, file.FileName);
        file.Category = category;

        var categoryDir = Path.Combine(root, category.ToString());

        // ===== 3. GROUP BY EXTENSION (OPTIONAL) =====
        var groupByExt =
            string.Equals(
                _settings.Get("group_by_extension"),
                "true",
                StringComparison.OrdinalIgnoreCase
            );

        if (!groupByExt)
        {
            return Path.Combine(categoryDir, file.FileName);
        }

        var ext = file.Extension.TrimStart('.').ToUpperInvariant();
        return Path.Combine(categoryDir, ext, file.FileName);
    }

}