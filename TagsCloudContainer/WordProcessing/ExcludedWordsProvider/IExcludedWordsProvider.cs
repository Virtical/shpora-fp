namespace TagsCloudContainer.ExcludedWordsProvider;

public interface IExcludedWordsProvider
{
    IEnumerable<string> GetExcludedWords();
}