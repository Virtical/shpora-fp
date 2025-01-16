namespace TagsCloudContainer;

public class ExcludedWordsSettings
{
    public string DefaultExcludedWordsPath { get; set; }
    public bool EnableDefaultExclude { get; set; }

    public ExcludedWordsSettings(string defaultExcludedWordsPath, bool enableDefaultExclude = true)
    {
        DefaultExcludedWordsPath = defaultExcludedWordsPath;
        EnableDefaultExclude = enableDefaultExclude;
    }
}