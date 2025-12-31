using DropSort.Core.Enums;

namespace Infrastructure.FileSystem;

public class TargetFolderInitializer
{
    public static void EnsureAll(string root)
    {
        foreach (var cat in Enum.GetNames<FileCategory>())
        {
            var dir = Path.Combine(root, cat);
            Directory.CreateDirectory(dir);
        }
    }
}