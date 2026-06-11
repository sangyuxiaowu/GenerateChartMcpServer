using System.Text.Json.Serialization;

namespace GenerateChartMcpServer;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(double[]))]
[JsonSerializable(typeof(ChartTools.SeriesData[]))]
[JsonSerializable(typeof(ChartTools.BubbleSeriesData[]))]
internal partial class GenerateChartJsonContext : JsonSerializerContext
{
}