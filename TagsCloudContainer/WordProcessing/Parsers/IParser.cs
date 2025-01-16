namespace TagsCloudContainer;

public interface IParser
{
    public List<string> GetWords(string filePath);
}