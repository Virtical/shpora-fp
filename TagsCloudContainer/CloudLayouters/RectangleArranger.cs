using System.Drawing;
using TagsCloudContainer.Extensions;

namespace TagsCloudContainer.CloudLayouters;

public class RectangleArranger
{
    private readonly List<Rectangle> _rectangles;
    private readonly ISpiral spiral;

    public RectangleArranger(ISpiral spiral)
    {
        _rectangles = [];
        this.spiral = spiral;
    }

    public Rectangle ArrangeRectangle(Size rectangleSize, Point center)
    {
        Rectangle newRectangle;

        do
        {
            var location = spiral.GetNextPoint();
            location.Offset(-rectangleSize.Width / 2, rectangleSize.Height / 2);
            newRectangle = new Rectangle(location, rectangleSize);
        }
        while (IsIntersectsWithAny(newRectangle));

        return ShiftRectangleToCenter(newRectangle, center);
    }

    private bool IsIntersectsWithAny(Rectangle rectangle)
    {
        return _rectangles.Any(existingRectangle => existingRectangle.IntersectsWith(rectangle));
    }

    private Rectangle ShiftRectangleToCenter(Rectangle rectangle, Point center)
    {
        var directionToCenter = rectangle.GetDirectionToCenter(center);
        while (directionToCenter != Point.Empty)
        {
            var nextRectangle = rectangle.Move(directionToCenter);
            if (IsIntersectsWithAny(nextRectangle))
                break;

            rectangle = nextRectangle;
            directionToCenter = rectangle.GetDirectionToCenter(center);
        }

        _rectangles.Add(rectangle);
        return rectangle;
    }
}