using TagsCloudContainer.WordProcessing;
using TagsCloudContainer.WordProcessing.Parsers;

namespace TagsCloudContainer;

public class WordProcessorFactory : IWordProcessorFactory
{
    private readonly ExcludedWordsSettings settings;

    public WordProcessorFactory(ExcludedWordsSettings settings)
    {
        this.settings = settings;
    }

    public WordProcessor Create()
    {
        var parsers = new Dictionary<string, IParser>
        {
            { ".txt", new ParserTxt() },
            { ".doc", new ParserDoc() },
            { ".docx", new ParserDoc() }
        };

        return new WordProcessor(parsers, settings);
    }
}