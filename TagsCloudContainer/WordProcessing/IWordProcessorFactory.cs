using TagsCloudContainer.WordProcessing;

namespace TagsCloudContainer;

public interface IWordProcessorFactory
{
    WordProcessor Create();
}