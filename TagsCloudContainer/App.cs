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
    private static string defaultWordsFilePath = Path.Combine("..", "..", "..", "DefaultWords", "cloud.txt");
    private static string defaultSaveFileName = "cloud.png";
    private static string defaultExcludedWordsFilePath = Path.Combine("..", "..", "..", "DefaultWords", "excluded_words.txt");
    
    private static readonly string[] validExtensions = [".png", ".jpeg", ".tiff", ".bmp"];

    private static readonly string imageDimensionsPrompt = $"Введите размер изображения (по умолчанию W: {defaultImageWidth}, H: {defaultImageHeight}):";
    private static readonly string fileNamePrompt = "Введите название файла с текстом:";
    private static readonly string saveFileNamePrompt = $"Введите название сохраняемого файла, по умолчанию {defaultSaveFileName}):";
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

            switch (input)
            {
                case ExitCommand:
                    Environment.Exit(0);
                    break;
                case DefaultCommand:
                    Console.Clear();
                    return defaultPath;
            }
            
            if (!string.IsNullOrEmpty(input) && File.Exists(input))
            {
                Console.Clear();
                return input;
            }

            Console.Clear();
            ShowExitMessage();
            Console.WriteLine("Файл не найден. Попробуйте снова.");
        }
    }
    
    public static string GetSaveFileNameFromUser()
    {
        ShowExitMessage();

        while (true)
        {
            Console.WriteLine(saveFileNamePrompt);
            Console.WriteLine($"(доступные форматы: [{string.Join(", ", validExtensions)}])");
            var input = Console.ReadLine();
            
            switch (input)
            {
                case ExitCommand:
                    Environment.Exit(0);
                    break;
                case DefaultCommand:
                    Console.Clear();
                    return defaultSaveFileName;
            }
            
            var extension = Path.GetExtension(input)?.ToLower();

            if (!string.IsNullOrEmpty(input) && Array.Exists(validExtensions, ext => ext.Equals(extension)))
            {
                Console.Clear();
                return input;
            }

            Console.Clear();
            ShowExitMessage();
            Console.WriteLine("Неверное название файла. Попробуйте снова.");
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

            if (TryParseColors(input, out var colors))
                return colors;
        }
    }
    
    private static bool TryParseColors(string? input, out (Color Primary, Color? Secondary) result)
    {
        result = default;

        var colorInputs = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (colorInputs is not { Length: > 0 })
        {
            ShowErrorMessage("Некорректное значение. Попробуйте снова.");
            return false;
        }
        
        var primaryResult = ParseColor(colorInputs[0].Trim());
        if (!primaryResult.IsSuccess)
        {
            ShowErrorMessage(primaryResult.Error);
            return false;
        }

        if (colorInputs.Length > 1)
        {
            var secondaryResult = ParseColor(colorInputs[1].Trim());
            if (!secondaryResult.IsSuccess)
            {
                ShowErrorMessage(secondaryResult.Error);
                return false;
            }

            Console.Clear();
            result = (primaryResult.Value, secondaryResult.Value);
            return true;
        }

        Console.Clear();
        result = (primaryResult.Value, null);
        return true;

    }
    
    private static void ShowErrorMessage(string error)
    {
        Console.Clear();
        ShowExitMessage();
        Console.WriteLine(error);
    }
    
    private static Result<Color> ParseColor(string input)
    {
        if (Enum.TryParse(input, true, out KnownColor knownColor))
        {
            return Result.Ok(Color.FromKnownColor(knownColor));
        }
        
        return Result.Fail<Color>($"Invalid color name: '{input}'");
    }
}