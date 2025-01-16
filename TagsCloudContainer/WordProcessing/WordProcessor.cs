using TagsCloudContainer.ExcludedWordsProvider;

namespace TagsCloudContainer.WordProcessing;

public class WordProcessor
{
    private readonly List<string> words = new();
    private readonly HashSet<string> excludedWords = new();
    private readonly Dictionary<string, IParser> parsers;
    private readonly ExcludedWordsSettings settings;

    public WordProcessor(Dictionary<string, IParser> handlers, ExcludedWordsSettings settings)
    {
        parsers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public Result<WordProcessor> GetWordsForCloud(string wordsPath)
    {
        if (!File.Exists(wordsPath))
            return Result.Fail<WordProcessor>("File does not exist.");

        var extension = Path.GetExtension(wordsPath).ToLower();
        
        if (parsers.TryGetValue(extension, out var parser))
        {
            words.AddRange(parser.GetWords(wordsPath));
            return Result.Ok(this);
        }

        return Result.Fail<WordProcessor>("No parser available for the given file extension.");
    }

    public Result<WordProcessor> ExcludeWords(IExcludedWordsProvider provider)
    {
        if (provider == null)
            return Result.Fail<WordProcessor>("ExcludedWordsProvider is null.");

        excludedWords.UnionWith(provider.GetExcludedWords());
        return Result.Ok(this);
    }

    public Result<WordProcessor> DisableDefaultExclude()
    {
        settings.EnableDefaultExclude = false;
        return Result.Ok(this);
    }

    public Result<Dictionary<string, int>> ToDictionary()
    {
        try
        {
            if (settings.EnableDefaultExclude)
            {
                string workingDirectory = Directory.GetCurrentDirectory();
                
                if (!File.Exists(settings.DefaultExcludedWordsPath))
                    return Result.Fail<Dictionary<string, int>>("Default excluded words file does not exist.");

                var defaultExcludedWords = File.ReadAllLines(settings.DefaultExcludedWordsPath)
                    .Select(word => word.ToLower())
                    .ToHashSet();

                excludedWords.UnionWith(defaultExcludedWords);
            }

            var result = words
                .Where(word => !excludedWords.Contains(word))
                .GroupBy(word => word)
                .ToDictionary(group => group.Key, group => group.Count());

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            return Result.Fail<Dictionary<string, int>>($"An error occurred: {ex.Message}");
        }
    }
}
