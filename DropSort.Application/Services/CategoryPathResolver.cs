using DropSort.Core.Interfaces;
using DropSort.Core.Models;

namespace Application.Services;

public class CategoryPathResolver : ICategoryPathResolver
{
    private readonly ISettingRepository _settings;

    public CategoryPathResolver(ISettingRepository settings)
    {
        _settings = settings;
    }

    public string ResolveTargetPath(FileTask task)
    {
        var root = _settings.Get("download_root")
                   ?? throw new InvalidOperationException("download_root is not configured");

        var folderName = task.Category.ToString();
        return Path.Combine(root, folderName, task.FileName);
    }
}