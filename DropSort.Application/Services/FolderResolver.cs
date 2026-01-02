using DropSort.Core.Enums;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;

namespace Application.Services;
using Microsoft.Extensions.Logging;


public class FolderResolver : IFolderResolver
{
    private readonly ISettingRepository _settings;
    private readonly IFileClassifier _classifier;
    private readonly IKeywordRuleMatcher _keywordRuleMatcher;
    private readonly ILogger<FolderResolver> _logger;

    public FolderResolver(
        ISettingRepository settings,
        IFileClassifier classifier,
        IKeywordRuleMatcher keywordRuleMatcher,
        ILogger<FolderResolver> logger)
    {
        _settings = settings;
        _classifier = classifier;
        _keywordRuleMatcher = keywordRuleMatcher;
        _logger = logger;
    }


    public string ResolveTargetPath(FileItem file)
    {
        var root = _settings.Get("download_root");
        if (string.IsNullOrWhiteSpace(root))
            throw new InvalidOperationException("download_root is not configured");

        // ===== KEYWORD RULE OVERRIDE =====
        var keywordFolder = _keywordRuleMatcher.TryResolve(file);
        if (keywordFolder != null)
        {
            var keywordPath = Path.Combine(root, keywordFolder, file.FileName);

            _logger.LogInformation(
                "Resolved target path by keyword rule for file {file}: {path}",
                file.FileName,
                keywordPath
            );

            return keywordPath;
        }

        // ===== NORMALIZE EXTENSION =====
        var ext = file.Extension;

        if (!string.IsNullOrWhiteSpace(ext) && !ext.StartsWith("."))
        {
            ext = "." + ext;
        }

        _logger.LogDebug(
            "Resolving folder for file {file}, extension={ext}",
            file.FileName,
            ext
        );

        // ===== CLASSIFY =====
        var category = _classifier.Classify(ext, file.FileName);
        file.Category = category;

        var categoryDir = Path.Combine(root, category.ToString());

        var groupByExt =
            string.Equals(
                _settings.Get("group_by_extension"),
                "true",
                StringComparison.OrdinalIgnoreCase
            );

        // ===== FINAL TARGET PATH =====
        string targetPath;

        if (!groupByExt)
        {
            targetPath = Path.Combine(categoryDir, file.FileName);
        }
        else
        {
            var extFolder = ext.TrimStart('.').ToUpperInvariant();
            targetPath = Path.Combine(categoryDir, extFolder, file.FileName);
        }

        _logger.LogInformation(
            "Resolved target path for file {file}: {path}",
            file.FileName,
            targetPath
        );

        return targetPath;
    }


}