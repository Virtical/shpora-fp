using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudContainer;
using TagsCloudContainer.CloudLayouters;
using FakeItEasy;

[TestFixture]
public class CircularCloudLayouterTests
{
    private CircularCloudLayouter layouter;

    [SetUp]
    public void SetUp()
    {
        var spiral = new ArchimedeanSpiral(Point.Empty);
        layouter = new CircularCloudLayouter(Point.Empty, spiral);
    }

    [Test]
    public void CreateView_ShouldReturnFail_WhenWidthOrHeightIsNonPositive()
    {
        var result = layouter.CreateView(0, -1);
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Width and height must be positive.");
    }

    [Test]
    public void CreateView_ShouldReturnOk_WhenWidthAndHeightArePositive()
    {
        var result = layouter.CreateView(100, 100);
        
        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().NotBeNull();
    }

    [Test]
    public void SetFontName_ShouldReturnFail_WhenFontNameIsNullOrWhiteSpace()
    {
        var result = layouter.SetFontName(" ");
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Font name cannot be null or whitespace.");
    }

    [Test]
    public void SetFontName_ShouldReturnOk_WhenFontNameIsValid()
    {
        var result = layouter.SetFontName("Arial");
        
        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().NotBeNull();
    }

    [Test]
    public void SetBackgroundColor_ShouldReturnOk_WithValidColor()
    {
        var color = (Color.Red, (Color?)Color.Blue);

        var result = layouter.SetBackgroundColor(color);
        
        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().NotBeNull();
    }

    [Test]
    public void PutTags_ShouldReturnFail_WhenWordsAreNull()
    {
        var result = layouter.PutTags(null);
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Words dictionary cannot be null or empty.");
    }

    [Test]
    public void PutTags_ShouldReturnFail_WhenWordsAreEmpty()
    {
        var result = layouter.PutTags(new Dictionary<string, int>());
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Words dictionary cannot be null or empty.");
    }

    [Test]
    public void PutTags_ShouldReturnOk_WhenWordsAreValid()
    {
        var words = new Dictionary<string, int>
        {
            { "hello", 10 },
            { "world", 5 }
        };
        
        var result = layouter.PutTags(words);
        
        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().NotBeNull();
    }

    [Test]
    public void PutNextTag_ShouldReturnFail_WhenTextIsNullOrWhiteSpace()
    {
        var result = layouter.PutNextTag(" ", 10);
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Text cannot be null or whitespace.");
    }

    [Test]
    public void PutNextTag_ShouldReturnFail_WhenCountIsLessThanOne()
    {
        var result = layouter.PutNextTag("word", 0);
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Count must be greater than zero.");
    }

    [Test]
    public void PutNextTag_ShouldReturnOk_WhenTextAndCountAreValid()
    {
        var spiral = A.Fake<ISpiral>();
        var layouter = new CircularCloudLayouter(new Point(0, 0), spiral);

        var fakeMeasurer = A.Fake<IRectangleMeasurer>();
        A.CallTo(() => fakeMeasurer.Measure(A<string>.Ignored, A<Font>.Ignored)).Returns(new Size(100, 50));
        layouter.RectangleMeasurer = fakeMeasurer;

        var result = layouter.PutNextTag("word", 10);

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().NotBe(default(Rectangle));
    }
    
    [Test]
    public void CreateView_ShouldFail_WhenTagsDoNotFitWithinDimensions()
    {
        layouter.PutNextTag("word", 100);
        layouter.PutNextTag("test", 100);

        var result = layouter.CreateView(50, 50);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tags do not fit within the specified dimensions.");
    }

    [Test]
    public void CreateView_ShouldSucceed_WhenTagsFitWithinDimensions()
    {
        layouter.PutNextTag("word", 10);
        layouter.PutNextTag("test", 10);

        var result = layouter.CreateView(500, 500);

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().NotBeNull();
    }
}
