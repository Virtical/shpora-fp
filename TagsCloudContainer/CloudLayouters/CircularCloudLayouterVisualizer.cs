using System.Drawing;
using System.Drawing.Drawing2D;
using TagsCloudContainer.CloudLayouters;

namespace TagsCloudContainer;

public class CircularCloudLayouterVisualizer
{
    private CircularCloudLayouter layouter;
    private Size size;

    public CircularCloudLayouterVisualizer(CircularCloudLayouter layouter, Size bitmapSize)
    {
        this.layouter = layouter;
        size = bitmapSize;
    }
    
    public Result<CircularCloudLayouterVisualizer> SaveImage(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Result.Fail<CircularCloudLayouterVisualizer>("File path cannot be null or whitespace.");
        
        try
        {
            using var bitmap = new Bitmap(size.Width, size.Height);
            using var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(layouter.BackgroundColor?.Primary ?? Color.White);

            if (layouter.BackgroundColor?.Secondary != null)
            {
                var bounds = new Rectangle(0, 0, size.Width, size.Height);
                
                using var gradientBrush = new LinearGradientBrush(
                    bounds,
                    layouter.BackgroundColor?.Primary ?? Color.White,
                    layouter.BackgroundColor?.Secondary ?? Color.White,
                    LinearGradientMode.Horizontal);

                graphics.FillRectangle(gradientBrush, bounds);
            }

            var centerBitmap = new Point(size.Width / 2, size.Height / 2);
            var offsetBitmap = new Point(centerBitmap.X - layouter.Center.X, centerBitmap.Y - layouter.Center.Y);

            foreach (var rectangle in layouter.Tags)
            {
                rectangle.Rectangle.Offset(offsetBitmap);

                if (layouter.TextColor?.Secondary != null)
                {
                    var gradientBrush = new LinearGradientBrush(
                        rectangle.Rectangle,
                        layouter.TextColor?.Primary ?? Color.Blue,
                        layouter.TextColor?.Secondary ?? Color.Red,
                        LinearGradientMode.Horizontal);
                    
                    graphics.DrawString(rectangle.Text, rectangle.Font, gradientBrush, rectangle.Rectangle);
                }
                else
                {
                    var brush = new SolidBrush(layouter.TextColor?.Primary ?? Color.Black);
                    graphics.DrawString(rectangle.Text, rectangle.Font, brush, rectangle.Rectangle);
                }
            }

            bitmap.Save(filePath);
            
            return Result.Ok(this);;
        }
        catch (Exception ex)
        {
            return Result.Fail<CircularCloudLayouterVisualizer>($"An error occurred while saving the image: {ex.Message}");
        }
    }
}