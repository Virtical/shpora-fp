using System.Drawing;

namespace TagsCloudContainer.Extensions;

public static class RectangleExtensions
{
    public static bool IsIntersectsWithAny(this IEnumerable<Rectangle> rectangles, Rectangle rectangle)
    {
        return rectangles.Any(tag => tag.IntersectsWith(rectangle));
    }
    
    public static Point GetDirectionToCenter(this Rectangle rectangle, Point center)
    {
        var rectangleCenter = new Point(
            rectangle.Left + rectangle.Width / 2,
            rectangle.Top - rectangle.Height / 2);

        return new Point(
            Math.Sign(center.X - rectangleCenter.X),
            Math.Sign(center.Y - rectangleCenter.Y)
        );
    }
    
    public static Rectangle Move(this Rectangle rectangle, Point direction)
    {
        return new Rectangle(
            new Point(rectangle.X + direction.X, rectangle.Y + direction.Y),
            rectangle.Size);
    }
}
