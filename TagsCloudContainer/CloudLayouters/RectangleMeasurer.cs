using System.Drawing;

namespace TagsCloudContainer.CloudLayouters;

public class RectangleMeasurer : IRectangleMeasurer
{
    private static  Bitmap bitmap;
    private static Graphics graphics;
    
    static RectangleMeasurer()
    {
        bitmap = new Bitmap(1, 1);
        graphics = Graphics.FromImage(bitmap);
    }
    public Size Measure(string text, Font font)
    {
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        var format = new StringFormat { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
        var rectangleFSize = graphics.MeasureString(text, font, int.MaxValue, format);
        return new Size((int)rectangleFSize.Width + 2, (int)rectangleFSize.Height + 2);
    }
}