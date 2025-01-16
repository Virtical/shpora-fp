using System.Drawing;

namespace TagsCloudContainer.CloudLayouters;

public interface IRectangleMeasurer
{
    Size Measure(string text, Font font);
}