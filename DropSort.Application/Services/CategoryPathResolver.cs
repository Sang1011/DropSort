using DropSort.Core.Interfaces;
using DropSort.Core.Models;

namespace Application.Services;

public class CategoryPathResolver : ICategoryPathResolver
{
    private readonly ISettingRepository _settings;
    private readonly IFileClassifier _classifier;

    public CategoryPathResolver(
        ISettingRepository settings,
        IFileClassifier classifier)
    {
        _settings = settings;
        _classifier = classifier;
    }

    public string ResolveTargetPath(FileItem file)
    {
        var root = _settings.Get("download_root")
                   ?? throw new InvalidOperationException("download_root is not configured");

        var category = _classifier.Classify(file.Extension, file.FileName);
        file.Category = category;

        var folderName = category.ToString();
        return Path.Combine(root, folderName, file.FileName);
    }
}