using Autofac;
using TagsCloudContainer.CloudLayouters;
using TagsCloudContainer.Settings;
using TagsCloudContainer.WordProcessing.ExcludedWordsProvider;

namespace TagsCloudContainer;

public static class Program
{
    public static void Main()
    {
        using var scope = ContainerConfig.Configure().BeginLifetimeScope();
        
        var cloudLayouter = scope.Resolve<CircularCloudLayouter>();
        var imageSettings = scope.Resolve<ImageSettings>();
        var fontSettings = scope.Resolve<FontSettings>();
        var colorSettings = scope.Resolve<ColorSettings>();
        var filesSettings = scope.Resolve<FilesSettings>();

        var wordProcessor = scope.Resolve<WordProcessorFactory>().Create();
        var fileExcludedWordsProvider = scope.Resolve<ExcludedWordsProviderFactory>().Create();
        
        var result = wordProcessor
            .GetWordsForCloud(filesSettings.Words)
            .Then(processor => processor.ExcludeWords(fileExcludedWordsProvider))
            .Then(processor => processor.DisableDefaultExclude())
            .Then(processor => processor.ToDictionary());
        
        if (!result.IsSuccess)
        {
            Console.WriteLine($"Error: {result.Error}");
            Environment.Exit(1);
        }

        var words = result.Value;

        cloudLayouter
            .SetFontName(fontSettings.FontName)
            .Then(layout => layout.SetBackgroundColor(colorSettings.BackgroundColor))
            .Then(layout => layout.SetTextColor(colorSettings.TextColor))
            .Then(layout => layout.PutTags(words))
            .Then(layout => layout.CreateView(imageSettings.Width, imageSettings.Height))
            .Then(layout => layout.SaveImage("cloud.jpeg"))
            .Then(layout => layout.SaveImage("cloud.png"))
            .Then(layout => layout.SaveImage("cloud.bmp"))
            .Then(layout => layout.SaveImage("cloud.tiff"));
    }
}