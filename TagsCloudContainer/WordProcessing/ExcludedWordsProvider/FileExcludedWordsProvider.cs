using TagsCloudContainer.ExcludedWordsProvider;

namespace TagsCloudContainer.WordProcessing.ExcludedWordsProvider;

public class FileExcludedWordsProvider : IExcludedWordsProvider
{
    private readonly string filePath;
    private readonly IDictionary<string, IParser> parsers;

    public FileExcludedWordsProvider(string filePath, IDictionary<string, IParser> parsers)
    {
        this.filePath = filePath;
        this.parsers = parsers;
    }

    public IEnumerable<string> GetExcludedWords()
    {
        if (!File.Exists(filePath))
            return [];

        var extension = Path.GetExtension(filePath).ToLower();
        return parsers.TryGetValue(extension, out var parser)
            ? parser.GetWords(filePath)
            : Enumerable.Empty<string>();
    }
}