using DropSort.Core.Enums;

namespace DropSort.Core.Interfaces;

public interface IFileClassifier
{
    FileCategory Classify(string extension, string fileName);
}