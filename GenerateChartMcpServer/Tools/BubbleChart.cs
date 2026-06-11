using System.ComponentModel;
using ModelContextProtocol.Server;
using ScottPlot;

namespace GenerateChartMcpServer;

internal partial class ChartTools
{
    [McpServerTool]
    [Description("生成气泡图。seriesData 格式：[{\"name\":\"系列A\",\"x\":[1,2,3],\"y\":[10,20,30],\"size\":[40,50,60]}]。返回签名图片 URL。")]
    public string GenerateBubbleChart(
        [Description("图表标题")] string title,
        [Description("气泡系列数据，JSON 数组")] string seriesData,
        [Description("X 轴标题")] string? xAxisTitle = null,
        [Description("Y 轴标题")] string? yAxisTitle = null,
        [Description("图片宽度")] int width = 800,
        [Description("图片高度")] int height = 600)
    {
        var series = ParseBubbleSeriesData(seriesData);
        return SaveAndSignUrl(width, height, plot =>
        {
            plot.Title(title);
            var colors = GenColors(series.Length);
            for (int i = 0; i < series.Length; i++)
            {
                var s = plot.Add.Scatter(series[i].X, series[i].Y);
                var r = Math.Max(4, Math.Min(40, series[i].Size.Length > 0 ? series[i].Size.Average() / 3 : 12));
                s.Color = colors[i].WithAlpha(0.6); s.LegendText = series[i].Name;
                s.MarkerSize = (float)r; s.LineWidth = 0;
            }
            plot.ShowLegend();
            if (!string.IsNullOrEmpty(xAxisTitle)) plot.Axes.Bottom.Label.Text = xAxisTitle;
            if (!string.IsNullOrEmpty(yAxisTitle)) plot.Axes.Left.Label.Text = yAxisTitle;
        });
    }
}
