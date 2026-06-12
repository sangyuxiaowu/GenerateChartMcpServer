using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Sang.AspNetCore.SignAuthorization;
using ScottPlot;
using SkiaSharp;

namespace GenerateChartMcpServer;

internal partial class ChartTools
{
    internal static SignAuthorizationOptions SignOptions = new();
    private static readonly Lazy<string?> ResolvedFontName = new(ResolveFontName);

    internal static string ImageBaseUrl { get; set; } = "http://localhost:52345/images";
    internal static string ImageStoragePath { get; set; } = "images";
    internal static int ImageExpireMonths { get; set; } = 1;
    internal static string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
    internal static string? ConfiguredFontName { get; set; }
    internal static string? FontName => ResolvedFontName.Value;

    private static readonly char[] MonthCodeLetters = "abcdefghijkl".ToCharArray();

    internal static string SaveAndSignUrl(int w, int h, Action<Plot> build)
    {
        var plot = new Plot();
        ApplyFont(plot);
        build(plot);
        var name = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Random.Shared.Next(1000, 9999)}.png";
        var dirName = GetImageDirectoryName(DateTimeOffset.UtcNow);
        var dir = Path.Combine(ContentRootPath, ImageStoragePath, dirName);
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, name);
        plot.SavePng(path, w, h);

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var nonce = Guid.NewGuid().ToString("N")[..8];
        var sign = MakeSignAuthorization.MakeSign(SignOptions.sToken, timestamp, nonce, $"/images/{dirName}/{name}");
        return $"{ImageBaseUrl}/images/{dirName}/{name}?timestamp={timestamp}&nonce={nonce}&signature={sign}";
    }

    internal static string GetImageDirectoryName(DateTimeOffset now)
    {
        var year = now.Year % 100;
        var monthIndex = Math.Clamp(now.Month - 1, 0, MonthCodeLetters.Length - 1);
        return $"{year:D2}{MonthCodeLetters[monthIndex]}";
    }

    internal static string[] ParseStrArr(string json) =>
        Deserialize(json, GenerateChartJsonContext.Default.StringArray)
            ?? throw new ArgumentException("Invalid JSON array.");

    internal static double[] ParseDblArr(string json) =>
        Deserialize(json, GenerateChartJsonContext.Default.DoubleArray)
            ?? throw new ArgumentException("Invalid JSON number array.");

    internal class SeriesData { public string Name { get; set; } = ""; public double[] Values { get; set; } = Array.Empty<double>(); }
    internal static SeriesData[] ParseSeriesData(string json) =>
        Deserialize(json, GenerateChartJsonContext.Default.SeriesDataArray)
            ?? throw new ArgumentException("Invalid series JSON.");

    internal class BubbleSeriesData { public string Name { get; set; } = ""; public double[] X { get; set; } = Array.Empty<double>(); public double[] Y { get; set; } = Array.Empty<double>(); public double[] Size { get; set; } = Array.Empty<double>(); }
    internal static BubbleSeriesData[] ParseBubbleSeriesData(string json) =>
        Deserialize(json, GenerateChartJsonContext.Default.BubbleSeriesDataArray)
            ?? throw new ArgumentException("Invalid bubble series JSON.");

    internal static Color[] GenColors(int n) =>
        Enumerable.Range(0, n).Select(i => Color.FromHSL((float)(i * 360.0 / n), 70, 60)).ToArray();

    private static void ApplyFont(Plot plot)
    {
        var fontName = FontName;
        if (string.IsNullOrWhiteSpace(fontName))
            return;

        try
        {
            plot.Font.Set(fontName);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[GenerateChartMcpServer] Failed to apply font '{fontName}': {ex.Message}");
        }
    }

    private static string? ResolveFontName()
    {
        if (!string.IsNullOrWhiteSpace(ConfiguredFontName))
            return ConfiguredFontName;

        try
        {
            return SKFontManager.Default.MatchCharacter('汉')?.FamilyName
                ?? SKFontManager.Default.MatchCharacter('A')?.FamilyName;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[GenerateChartMcpServer] Failed to resolve system font: {ex.Message}");
            return null;
        }
    }

    private static T? Deserialize<T>(string json, JsonTypeInfo<T> typeInfo)
    {
        try
        {
            return JsonSerializer.Deserialize(json, typeInfo);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON: {ex.Message}", ex);
        }
    }
}
