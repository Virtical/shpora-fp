using System.Drawing;

namespace TagsCloudContainer.Extensions;

public static class TagExtensions
{
    public static Size CalculateSize(this List<Tag> tags)
    {
        if (tags.Count == 0)
            return Size.Empty;

        var left = tags.Select(tag => tag.Rectangle).Min(rectangle => rectangle.Left);
        var right = tags.Select(tag => tag.Rectangle).Max(rectangle => rectangle.Right);
        var top = tags.Select(tag => tag.Rectangle).Min(rectangle => rectangle.Top);
        var bottom = tags.Select(tag => tag.Rectangle).Max(rectangle => rectangle.Bottom);

        return new Size(right - left, bottom - top);
    }
}