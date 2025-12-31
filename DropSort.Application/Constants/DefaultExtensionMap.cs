using DropSort.Core.Enums;

namespace Application.Constants;

public static class DefaultExtensionMap
{
    public static readonly Dictionary<string, FileCategory> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // 1. Documents
            [".pdf"] = FileCategory.Documents,
            [".doc"] = FileCategory.Documents,
            [".docx"] = FileCategory.Documents,
            [".xls"] = FileCategory.Documents,
            [".xlsx"] = FileCategory.Documents,
            [".csv"] = FileCategory.Documents,
            [".odt"] = FileCategory.Documents,
            [".ods"] = FileCategory.Documents,
            [".ppt"] = FileCategory.Documents,
            [".pptx"] = FileCategory.Documents,
            [".txt"] = FileCategory.Documents,
            [".rtf"] = FileCategory.Documents,
            [".md"] = FileCategory.Documents,

            // 2. Images
            [".jpg"] = FileCategory.Images,
            [".jpeg"] = FileCategory.Images,
            [".png"] = FileCategory.Images,
            [".webp"] = FileCategory.Images,
            [".bmp"] = FileCategory.Images,
            [".tiff"] = FileCategory.Images,
            [".svg"] = FileCategory.Images,
            [".ico"] = FileCategory.Images,
            [".gif"] = FileCategory.Images,

            // 3. Videos
            [".mp4"] = FileCategory.Videos,
            [".mkv"] = FileCategory.Videos,
            [".avi"] = FileCategory.Videos,
            [".mov"] = FileCategory.Videos,
            [".wmv"] = FileCategory.Videos,
            [".flv"] = FileCategory.Videos,
            [".webm"] = FileCategory.Videos,

            // 4. Audio
            [".mp3"] = FileCategory.Audio,
            [".wav"] = FileCategory.Audio,
            [".aac"] = FileCategory.Audio,
            [".ogg"] = FileCategory.Audio,
            [".flac"] = FileCategory.Audio,
            [".m4a"] = FileCategory.Audio,

            // 5. Archives
            [".zip"] = FileCategory.Archives,
            [".rar"] = FileCategory.Archives,
            [".7z"] = FileCategory.Archives,
            [".tar"] = FileCategory.Archives,
            [".gz"] = FileCategory.Archives,
            [".bz2"] = FileCategory.Archives,
            [".iso"] = FileCategory.Archives,

            // 6. Programs
            [".exe"] = FileCategory.Programs,
            [".msi"] = FileCategory.Programs,
            [".bat"] = FileCategory.Programs,
            [".cmd"] = FileCategory.Programs,
            [".appx"] = FileCategory.Programs,
            [".appxbundle"] = FileCategory.Programs,

            // 7. Dev
            [".js"] = FileCategory.Dev,
            [".ts"] = FileCategory.Dev,
            [".jsx"] = FileCategory.Dev,
            [".tsx"] = FileCategory.Dev,
            [".html"] = FileCategory.Dev,
            [".css"] = FileCategory.Dev,
            [".scss"] = FileCategory.Dev,
            [".py"] = FileCategory.Dev,
            [".java"] = FileCategory.Dev,
            [".cs"] = FileCategory.Dev,
            [".cpp"] = FileCategory.Dev,
            [".c"] = FileCategory.Dev,
            [".h"] = FileCategory.Dev,
            [".go"] = FileCategory.Dev,
            [".rs"] = FileCategory.Dev,
            [".php"] = FileCategory.Dev,
            [".json"] = FileCategory.Dev,
            [".xml"] = FileCategory.Dev,
            [".yml"] = FileCategory.Dev,
            [".yaml"] = FileCategory.Dev,
            [".env"] = FileCategory.Dev,
            [".sql"] = FileCategory.Dev,

            // 8. MediaSource
            [".psd"] = FileCategory.MediaSource,
            [".ai"] = FileCategory.MediaSource,
            [".xd"] = FileCategory.MediaSource,
            [".fig"] = FileCategory.MediaSource,
            [".blend"] = FileCategory.MediaSource,

            // 9. Mobile
            [".apk"] = FileCategory.Mobile,
            [".aab"] = FileCategory.Mobile,
            [".ipa"] = FileCategory.Mobile,

            // 10. Data
            [".db"] = FileCategory.Data,
            [".sqlite"] = FileCategory.Data,
            [".parquet"] = FileCategory.Data,
            [".feather"] = FileCategory.Data,

            // 11. Config
            [".ini"] = FileCategory.Config,
            [".cfg"] = FileCategory.Config,
            [".conf"] = FileCategory.Config,
            [".log"] = FileCategory.Config,
            [".tmp"] = FileCategory.Config,
        };
}