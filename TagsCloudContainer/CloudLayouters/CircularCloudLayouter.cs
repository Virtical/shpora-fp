using System.Drawing;
using TagsCloudContainer.Extensions;

namespace TagsCloudContainer.CloudLayouters;

public class CircularCloudLayouter
{
    public readonly Point Center;
    public List<Tag> Tags { get; }
    public (Color Primary, Color? Secondary)? BackgroundColor { get; private set; }
    public (Color Primary, Color? Secondary)? TextColor { get; private set; }
    public IRectangleMeasurer RectangleMeasurer { get; set; }
    
    private string? FontName { get; set; }
    private readonly RectangleArranger arranger;

    public CircularCloudLayouter(Point center, ISpiral spiral)
    {
        Center = center;
        Tags = [];
        arranger = new RectangleArranger(spiral);
        RectangleMeasurer = new RectangleMeasurer();
    }

    public Result<CircularCloudLayouterVisualizer> CreateView(int width, int height)
    {
        if (width <= 0 || height <= 0)
            return Result.Fail<CircularCloudLayouterVisualizer>("Width and height must be positive.");
        
        var layoutSize = Tags.CalculateSize();
        if (layoutSize.Width > width || layoutSize.Height > height)
            return Result.Fail<CircularCloudLayouterVisualizer>("Tags do not fit within the specified dimensions.");

        return Result.Ok(new CircularCloudLayouterVisualizer(this, new Size(width, height)));
    }

    public Result<CircularCloudLayouter> SetFontName(string fontName)
    {
        if (string.IsNullOrWhiteSpace(fontName))
            return Result.Fail<CircularCloudLayouter>("Font name cannot be null or whitespace.");

        FontName = fontName;
        return Result.Ok(this);
    }

    public Result<CircularCloudLayouter> SetBackgroundColor((Color Primary, Color? Secondary) color)
    {
        BackgroundColor = color;
        return Result.Ok(this);
    }
    
    public Result<CircularCloudLayouter> SetTextColor((Color Primary, Color? Secondary) color)
    {
        TextColor = color;
        return Result.Ok(this);
    }

    public Result<CircularCloudLayouter> PutTags(Dictionary<string, int>? words)
    {
        if (words is null || words.Count == 0)
            return Result.Fail<CircularCloudLayouter>("Words dictionary cannot be null or empty.");
        
        var sortedWords = words.OrderByDescending(pair => pair.Value);

        foreach (var word in sortedWords)
        {
            var tagResult = PutNextTag(word.Key, word.Value);
            
            if (!tagResult.IsSuccess)
                return Result.Fail<CircularCloudLayouter>(tagResult.Error);
        }
        
        return Result.Ok(this);
    }

    public Result<Rectangle> PutNextTag(string text, int sizeCoefficient)
    {
        if (sizeCoefficient < 1)
            return Result.Fail<Rectangle>("Count must be greater than zero.");

        if (string.IsNullOrWhiteSpace(text))
            return Result.Fail<Rectangle>("Text cannot be null or whitespace.");

        try
        {
            var font = new Font(FontName ?? "Arial", sizeCoefficient * 6 + 10);
            var rectangleSize = RectangleMeasurer.Measure(text, font);

            var rectangle = arranger.ArrangeRectangle(rectangleSize, Center);
            var tag = new Tag(text, font, rectangle);

            Tags.Add(tag);
            return Result.Ok(rectangle);
        }
        catch (Exception ex)
        {
            return Result.Fail<Rectangle>($"An error occurred while adding a tag: {ex.Message}");
        }
    }
}