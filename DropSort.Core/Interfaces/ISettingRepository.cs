namespace DropSort.Core.Interfaces;

public interface ISettingRepository
{
    void Set(string key, string value);
    string? Get(string key);
}