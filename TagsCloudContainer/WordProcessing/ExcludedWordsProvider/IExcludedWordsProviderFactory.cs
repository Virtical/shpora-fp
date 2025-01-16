namespace TagsCloudContainer.WordProcessing.ExcludedWordsProvider;

public interface IExcludedWordsProviderFactory
{
    FileExcludedWordsProvider Create();
}