using DropSort.Core.Interfaces;

namespace Infrastructure.FileSystem;

public class DuplicateResolver : IDuplicateResolver
{
    public string Resolve(string targetPath)
    {
        if (!File.Exists(targetPath))
            return targetPath;

        var dir = Path.GetDirectoryName(targetPath)!;
        var name = Path.GetFileNameWithoutExtension(targetPath);
        var ext  = Path.GetExtension(targetPath);

        var stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return Path.Combine(dir, $"{name}_{stamp}{ext}");
    }
}