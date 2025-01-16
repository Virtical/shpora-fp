using System.Drawing;

namespace TagsCloudContainer.Settings;

public class ColorSettings
{
    public (Color, Color?) BackgroundColor { get; }
    public (Color, Color?) TextColor { get; }

    public ColorSettings((Color, Color?) backgroundColor, (Color, Color?) textColor)
    {
        BackgroundColor = backgroundColor;
        TextColor = textColor;
    }
}