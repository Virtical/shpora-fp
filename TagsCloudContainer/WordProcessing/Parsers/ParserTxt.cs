namespace TagsCloudContainer.WordProcessing.Parsers;

public class ParserTxt : IParser
{
    public List<string> GetWords(string filePath)
    {
        return File.ReadAllLines(filePath)
            .Select(word => word.ToLower())
            .ToList();
    }
}