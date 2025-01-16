using Aspose.Words;

namespace TagsCloudContainer.WordProcessing.Parsers;

public class ParserDoc : IParser
{
    public List<string> GetWords(string filePath)
    {
        var doc = new Document(filePath);
        var content = doc.GetText();
        return content.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLower())
            .Skip(1)
            .SkipLast(1)
            .ToList();
    }
}