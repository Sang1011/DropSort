using DropSort.Core.Enums;
using DropSort.Core.Interfaces;
using Application.Constants;

namespace Application.Services;

public class FileClassifier : IFileClassifier
{
    public FileCategory Classify(string extension, string fileName)
    {
        if (DefaultExtensionMap.Map.TryGetValue(extension, out var category))
            return category;

        return FileCategory.Others;
    }
}