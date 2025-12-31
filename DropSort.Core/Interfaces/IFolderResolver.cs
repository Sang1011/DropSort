using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface IFolderResolver
{
    string ResolveTargetPath(FileItem file);
}