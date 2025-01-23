using Autofac;
using TagsCloudContainer.CloudLayouters;
using TagsCloudContainer.Settings;
using TagsCloudContainer.WordProcessing.ExcludedWordsProvider;

namespace TagsCloudContainer;

public static class Program
{
    public static void Main()
    {
        try
        {
            var containerResult = ContainerConfig.Configure();
            if (!containerResult.IsSuccess)
            {
                Console.WriteLine($"Configuration failed: {containerResult.Error}");
                return;
            }

            using var scope = containerResult.Value.BeginLifetimeScope();

            var cloudLayouter = scope.Resolve<CircularCloudLayouter>();
            var imageSettings = scope.Resolve<ImageSettings>();
            var fontSettings = scope.Resolve<FontSettings>();
            var colorSettings = scope.Resolve<ColorSettings>();
            var filesSettings = scope.Resolve<FilesSettings>();

            var wordProcessor = scope.Resolve<WordProcessorFactory>().Create();
            var fileExcludedWordsProvider = scope.Resolve<ExcludedWordsProviderFactory>().Create();

            var resultWordProcessor = wordProcessor
                .GetWordsForCloud(filesSettings.Words)
                .Then(processor => processor.ExcludeWords(fileExcludedWordsProvider))
                .Then(processor => processor.DisableDefaultExclude())
                .Then(processor => processor.ToDictionary());

            if (!resultWordProcessor.IsSuccess)
            {
                Console.WriteLine($"Error: {resultWordProcessor.Error}");
                Environment.Exit(1);
            }

            var words = resultWordProcessor.Value;

            var resultCloudLayouter = cloudLayouter
                .SetFontName(fontSettings.FontName)
                .Then(layout => layout.SetBackgroundColor(colorSettings.BackgroundColor))
                .Then(layout => layout.SetTextColor(colorSettings.TextColor))
                .Then(layout => layout.PutTags(words))
                .Then(layout => layout.CreateView(imageSettings.Width, imageSettings.Height))
                .Then(layout => layout.SaveImage(filesSettings.SaveFileName));

            if (!resultCloudLayouter.IsSuccess)
            {
                Console.WriteLine($"Error: {resultCloudLayouter.Error}");
                Environment.Exit(1);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            Environment.Exit(1);
        }
    }
}