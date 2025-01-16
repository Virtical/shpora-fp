using FluentAssertions;
using NUnit.Framework;
using TagsCloudContainer.ExcludedWordsProvider;
using TagsCloudContainer.WordProcessing;
using FakeItEasy;

namespace TagsCloudContainer.Tests;

[TestFixture]
public class WordProcessorTests
{
    private Dictionary<string, IParser> parsers;
    private ExcludedWordsSettings settings;
    private WordProcessor wordProcessor;

    [SetUp]
    public void SetUp()
    {
        parsers = new Dictionary<string, IParser>();
        settings = new ExcludedWordsSettings(Path.Combine("..", "..", "..", "excluded_words.txt"));
        wordProcessor = new WordProcessor(parsers, settings);
    }

    [Test]
    public void GetWordsForCloud_ShouldReturnFail_WhenFileDoesNotExist()
    {
        const string fakePath = "nonexistent.txt";
        
        var result = wordProcessor.GetWordsForCloud(fakePath);
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("File does not exist.");
    }

    [Test]
    public void GetWordsForCloud_ShouldReturnFail_WhenNoParserAvailable()
    {
        var fakePath = "file.unknown";
        File.WriteAllText(fakePath, "content");

        var result = wordProcessor.GetWordsForCloud(fakePath);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No parser available for the given file extension.");

        File.Delete(fakePath);
    }

    [Test]
    public void GetWordsForCloud_ShouldReturnOk_WhenParserAvailable()
    {
        var fakePath = "file.txt";
        var fakeParser = A.Fake<IParser>();
        A.CallTo(() => fakeParser.GetWords(fakePath)).Returns(new List<string> { "word1", "word2" });

        parsers[".txt"] = fakeParser;
        File.WriteAllText(fakePath, "content");

        var result = wordProcessor.GetWordsForCloud(fakePath);

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().NotBeNull();

        File.Delete(fakePath);
    }

    [Test]
    public void ExcludeWords_ShouldReturnFail_WhenProviderIsNull()
    {
        var result = wordProcessor.ExcludeWords(null);
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("ExcludedWordsProvider is null.");
    }

    [Test]
    public void ExcludeWords_ShouldReturnOk_WhenProviderIsValid()
    {
        var fakeProvider = A.Fake<IExcludedWordsProvider>();
        A.CallTo(() => fakeProvider.GetExcludedWords()).Returns(new List<string> { "word1", "word2" });

        var result = wordProcessor.ExcludeWords(fakeProvider);

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().NotBeNull();
    }

    [Test]
    public void ToDictionary_ShouldReturnFail_WhenDefaultExcludedWordsFileDoesNotExist()
    {
        settings.DefaultExcludedWordsPath = "missing.txt";
        
        var result = wordProcessor.ToDictionary();
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Default excluded words file does not exist.");
    }

    [Test]
    public void ToDictionary_ShouldReturnOk_WithCorrectWordCounts()
    {
        var fakeProvider = A.Fake<IExcludedWordsProvider>();
        A.CallTo(() => fakeProvider.GetExcludedWords()).Returns(new List<string> { "exclude" });

        wordProcessor.GetWordsForCloud("cloud.txt");
        wordProcessor.ExcludeWords(fakeProvider);
        
        var result = wordProcessor.ToDictionary();
        
        result.GetValueOrThrow().Should().BeEmpty();
        result.IsSuccess.Should().BeTrue();
    }
}