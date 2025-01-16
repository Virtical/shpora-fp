using TagsCloudContainer.WordProcessing.Parsers;

namespace TagsCloudContainer.WordProcessing.ExcludedWordsProvider;

public class ExcludedWordsProviderFactory : IExcludedWordsProviderFactory
{
    private readonly string filePath;

    public ExcludedWordsProviderFactory(string filePath)
    {
        this.filePath = filePath;
    }

    public FileExcludedWordsProvider Create()
    {
        var parsers = new Dictionary<string, IParser>
        {
            { ".txt", new ParserTxt() },
            { ".doc", new ParserDoc() },
            { ".docx", new ParserDoc() }
        };
        
        return new FileExcludedWordsProvider(filePath, parsers);
    }
}