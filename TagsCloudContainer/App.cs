using System.Drawing;
using TagsCloudContainer.Extensions;

namespace TagsCloudContainer;

public static class App
{
    private const string DefaultCommand = "default";
    private const string ExitCommand = "exit";

    private static int defaultImageWidth = 1000;
    private static int defaultImageHeight = 1000;
    private static string defaultFontName = "Arial";
    private static Color defaultBackgroundColor = Color.White;
    private static Color defaultTextColor = Color.Black;
    private static string defaultWordsFilePath = Path.Combine("..", "..", "..", "WordProcessing", "cloud.txt");
    private static string defaultExcludedWordsFilePath = Path.Combine("..", "..", "..", "WordProcessing", "excluded_words.txt");

    private static readonly string imageDimensionsPrompt = $"Введите размер изображения (по умолчанию W: {defaultImageWidth}, H: {defaultImageHeight}):";
    private static readonly string fileNamePrompt = "Введите название файла с текстом:";
    private static readonly string excludedWordsFileNamePrompt = "Введите название файла с исключёнными словами:";
    private static readonly string fontNamePrompt = $"Введите название шрифта (по умолчанию {defaultFontName}):";
    private static readonly string backgroundColorPrompt = $"Введите цвет фона (по умолчанию {defaultBackgroundColor.Name}):";
    private static readonly string textColorPrompt = $"Введите цвет текста (по умолчанию {defaultTextColor.Name}):";

    public static string GetDefaultExcludedWordsFilePath()
    {
        return defaultExcludedWordsFilePath;
    }

    private static void ShowExitMessage()
    {
        Console.WriteLine($"Чтобы выйти из программы, напишите \"{ExitCommand}\", чтобы использовать значение по умолчанию, напишите \"{DefaultCommand}\"");
        Console.WriteLine();
    }
    
    public static string GetFileNameFromUser()
    {
        return GetFileName(fileNamePrompt, defaultWordsFilePath);
    }

    public static string GetExcludedWordsFileNameFromUser()
    {
        return GetFileName(excludedWordsFileNamePrompt, string.Empty);
    }

    private static string GetFileName(string prompt, string defaultPath)
    {
        ShowExitMessage();

        while (true)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();

            if (!string.IsNullOrEmpty(input) && File.Exists(input))
            {
                Console.Clear();
                return input;
            }

            switch (input)
            {
                case ExitCommand:
                    Environment.Exit(0);
                    break;
                case DefaultCommand:
                    Console.Clear();
                    return defaultPath;
            }

            Console.Clear();
            ShowExitMessage();
            Console.WriteLine("Файл не найден. Попробуйте снова.");
        }
    }
    
    public static ImageDimensions GetImageDimensionsFromUser()
    {
        ShowExitMessage();
        
        while (true)
        {
            Console.WriteLine(imageDimensionsPrompt);
            Console.WriteLine("(в формате \"ширина высота\")");
            var input = Console.ReadLine();
            var size = input?.Split(' ');

            if (size?.Length == 2 &&
                int.TryParse(size[0], out var width) &&
                int.TryParse(size[1], out var height) &&
                width > 0 && height > 0)
            {
                Console.Clear();
                return new ImageDimensions(width, height);
            }
            
            switch (input)
            {
                case ExitCommand:
                    Environment.Exit(0);
                    break;
                case DefaultCommand:
                    Console.Clear();
                    return new ImageDimensions(defaultImageWidth, defaultImageHeight);
            }
            
            Console.Clear();
            ShowExitMessage();
            Console.WriteLine("Некорректный ввод. Убедитесь, что вы ввели два положительных целых числа.");
        }
    }
    
    public static string GetFontNameFromUser()
    {
        ShowExitMessage();
        
        while (true)
        {
            Console.WriteLine(fontNamePrompt);
            var input = Console.ReadLine();

            if (input!.FontExists())
            {
                Console.Clear();
                return input!;
            }

            switch (input)
            {
                case ExitCommand:
                    Environment.Exit(0);
                    break;
                case DefaultCommand:
                    Console.Clear();
                    return defaultFontName;
            }
            
            Console.Clear();
            ShowExitMessage();
            Console.WriteLine("Шрифт не найден. Попробуйте снова.");
        }
    }

    public static (Color Primary, Color? Secondary) GetBackgroundColorsFromUser()
    {
        return GetColorsFromUser(backgroundColorPrompt, defaultBackgroundColor);
    }
    
    public static (Color Primary, Color? Secondary) GetTextColorsFromUser()
    {
        return GetColorsFromUser(textColorPrompt, defaultTextColor);
    }
    
    private static (Color Primary, Color? Secondary) GetColorsFromUser(string prompt, Color defaultColor)
    {
        ShowExitMessage();

        while (true)
        {
            Console.WriteLine(prompt);
            Console.WriteLine("(Чтобы использовать градиент введите 2 цвета через пробел)");
            var input = Console.ReadLine();
            
            switch (input)
            {
                case ExitCommand:
                    Environment.Exit(0);
                    break;
                case DefaultCommand:
                    Console.Clear();
                    return (defaultColor, null);
            }
            
            var colorInputs = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (colorInputs is { Length: > 0 })
            {
                try
                {
                    var primaryColor = ParseColor(colorInputs[0].Trim());
                    
                    if (colorInputs.Length > 1)
                    {
                        var secondaryColor = ParseColor(colorInputs[1].Trim());
                        Console.Clear();
                        return (primaryColor, secondaryColor);
                    }
                    
                    Console.Clear();
                    return (primaryColor, null);
                }
                catch (ArgumentException ex)
                {
                    Console.Clear();
                    ShowExitMessage();
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }

            Console.Clear();
            ShowExitMessage();
            Console.WriteLine("Некорректное значение. Попробуйте снова.");
        }
    }
    
    private static Color ParseColor(string input)
    {
        if (Enum.TryParse(input, true, out KnownColor knownColor))
        {
            return Color.FromKnownColor(knownColor);
        }

        throw new ArgumentException($"Некорректное название цвета: {input}");
    }
}