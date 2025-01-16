using System.Drawing;

namespace TagsCloudContainer.Extensions;

public static class FontExtensions
{
    public static bool FontExists(this string fontName)
    {
        return FontFamily.Families.Any(font => font.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));
    }
}