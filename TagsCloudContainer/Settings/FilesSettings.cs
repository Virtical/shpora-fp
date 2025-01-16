namespace TagsCloudContainer.Settings;

public class FilesSettings
{
    public string Words { get; }
    public string ExcludedWords { get; }

    public FilesSettings(string words, string excludedWords)
    {
        Words = words;
        ExcludedWords = excludedWords;
    }
}