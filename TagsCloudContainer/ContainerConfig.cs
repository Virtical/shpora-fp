using Autofac;
using TagsCloudContainer.CloudLayouters;
using TagsCloudContainer.Settings;
using TagsCloudContainer.WordProcessing.ExcludedWordsProvider;
using static TagsCloudContainer.App;

namespace TagsCloudContainer;

public static class ContainerConfig
{
    public static Result<IContainer> Configure()
    {
        var builder = new ContainerBuilder();

        try
        {
            var imageDimensions = GetImageDimensionsFromUser();
            builder.RegisterInstance(new ImageSettings(imageDimensions.Width, imageDimensions.Height)).AsSelf();

            var fontName = GetFontNameFromUser();
            builder.RegisterInstance(new FontSettings(fontName)).AsSelf();

            var backgroundColor = GetBackgroundColorsFromUser();
            var textColor = GetTextColorsFromUser();
            builder.RegisterInstance(new ColorSettings(backgroundColor, textColor)).AsSelf();

            var wordsFileName = GetFileNameFromUser();
            var excludedWordsFileName = GetExcludedWordsFileNameFromUser();
            var saveFileName = GetSaveFileNameFromUser();
            builder.RegisterInstance(new FilesSettings(wordsFileName, excludedWordsFileName, saveFileName)).AsSelf();

            builder.RegisterType<ArchimedeanSpiral>()
                .As<ISpiral>()
                .WithParameter("center", imageDimensions.Center)
                .WithParameter("step", 1.0);

            var defaultExcludedWordsPath = GetDefaultExcludedWordsFilePath();
            builder.RegisterType<ExcludedWordsProviderFactory>().AsSelf()
                .WithParameter("filePath", excludedWordsFileName);
            builder.RegisterType<WordProcessorFactory>().AsSelf()
                .WithParameter("settings", new ExcludedWordsSettings(defaultExcludedWordsPath));
            builder.Register(c => new CircularCloudLayouter(imageDimensions.Center, c.Resolve<ISpiral>()))
                .AsSelf();

            return Result.Ok(builder.Build());
        }
        catch (Exception ex)
        {
            return Result.Fail<IContainer>($"An error occurred during container configuration: {ex.Message}");
        }
    }
}