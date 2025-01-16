using System.Drawing;

namespace TagsCloudContainer;

public class Tag
{
    public string Text { get; }
    public Font Font { get; }
    public Rectangle Rectangle { get; }

    public Tag(string text, Font font, Rectangle rectangle)
    {
        Text = text;
        Font = font;
        Rectangle = rectangle;
    }
}