namespace TagsCloudContainer.Settings;

public class FilesSettings
{
    public string Words { get; }
    public string ExcludedWords { get; }
    public string SaveFileName { get; }
    

    public FilesSettings(string words, string excludedWords, string saveFileName)
    {
        Words = words;
        ExcludedWords = excludedWords;
        SaveFileName = saveFileName;
    }
}