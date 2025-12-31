using System.Text;
using System.Text.RegularExpressions;
using DropSort.Core.Interfaces;

namespace Application.Services;

public class RenameService : IRenameService
{
    public string Normalize(string fileName)
    {
        var lower = fileName.ToLowerInvariant();

        lower = Regex.Replace(lower, @"\s+", "_");

        var normalized = lower.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}