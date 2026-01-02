using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;


public interface ICategoryPathResolver
{
    string ResolveTargetPath(FileTask task);
}