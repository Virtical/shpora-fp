using System.Drawing;

namespace TagsCloudContainer;

public class ImageDimensions
{
    public readonly int Width;
    public readonly int Height;
    public readonly Point Center;

    public ImageDimensions(int width, int height)
    {
        Width = width;
        Height = height;
        Center = new Point(Width / 2, Height / 2);
    }
}