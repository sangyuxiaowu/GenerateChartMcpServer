using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenerateChartMcpServer;

internal sealed class ImageCleanupService : BackgroundService
{
    private static readonly char[] MonthCodeLetters = "abcdefghijkl".ToCharArray();

    private readonly ILogger<ImageCleanupService> _logger;
    private readonly string _baseImageDir;
    private readonly int _expireMonths;

    public ImageCleanupService(ILogger<ImageCleanupService> logger)
    {
        _logger = logger;
        _baseImageDir = Path.Combine(ChartTools.ContentRootPath, ChartTools.ImageStoragePath);
        _expireMonths = ChartTools.ImageExpireMonths;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_expireMonths <= 0)
        {
            _logger.LogInformation("Image cleanup disabled because ImageExpireMonths is set to 0.");
            await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                DeleteExpiredDirectories();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clean up expired image directories.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken).ConfigureAwait(false);
        }
    }

    private void DeleteExpiredDirectories()
    {
        if (!Directory.Exists(_baseImageDir))
            return;

        var threshold = DateTimeOffset.UtcNow.AddMonths(-_expireMonths);
        foreach (var directoryPath in Directory.EnumerateDirectories(_baseImageDir, "*", SearchOption.TopDirectoryOnly))
        {
            try
            {
                var directoryName = Path.GetFileName(directoryPath);
                var directoryStart = ParseDirectoryDate(directoryName);
                if (directoryStart == null)
                    continue;

                var directoryEnd = directoryStart.Value.AddMonths(1).AddTicks(-1);
                if (directoryEnd < threshold)
                {
                    Directory.Delete(directoryPath, true);
                    _logger.LogInformation("Deleted expired image directory: {DirectoryPath}", directoryPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not delete image directory: {DirectoryPath}", directoryPath);
            }
        }
    }

    private static DateTimeOffset? ParseDirectoryDate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length != 3)
            return null;

        if (!int.TryParse(name.Substring(0, 2), out var year2))
            return null;

        var letter = char.ToLowerInvariant(name[2]);
        var monthIndex = Array.IndexOf(MonthCodeLetters, letter);
        if (monthIndex < 0)
            return null;

        var year = 2000 + year2;
        return new DateTimeOffset(year, monthIndex + 1, 1, 0, 0, 0, TimeSpan.Zero);
    }
}
