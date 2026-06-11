using System.ComponentModel;
using ModelContextProtocol.Server;
using ScottPlot;

namespace GenerateChartMcpServer;

internal partial class ChartTools
{
    [McpServerTool]
    [Description("生成折线图。xLabels 为 X 轴标签 JSON 数组，seriesData 格式同柱状图。返回签名图片 URL。")]
    public string GenerateLineChart(
        [Description("图表标题")] string title,
        [Description("X 轴标签，JSON 字符串数组")] string xLabels,
        [Description("系列数据，JSON 数组")] string seriesData,
        [Description("X 轴标题")] string? xAxisTitle = null,
        [Description("Y 轴标题")] string? yAxisTitle = null,
        [Description("图片宽度")] int width = 800,
        [Description("图片高度")] int height = 600)
    {
        var labels = ParseStrArr(xLabels);
        var series = ParseSeriesData(seriesData);
        return SaveAndSignUrl(width, height, plot =>
        {
            plot.Title(title);
            var xVals = Enumerable.Range(1, labels.Length).Select(x => (double)x).ToArray();
            var colors = GenColors(series.Length);
            for (int i = 0; i < series.Length; i++)
            {
                var s = plot.Add.Scatter(xVals, series[i].Values);
                s.Color = colors[i]; s.LegendText = series[i].Name; s.LineWidth = 2; s.MarkerSize = 8;
            }
            plot.ShowLegend();
            plot.Axes.Bottom.SetTicks(xVals, labels);
            if (!string.IsNullOrEmpty(xAxisTitle)) plot.Axes.Bottom.Label.Text = xAxisTitle;
            if (!string.IsNullOrEmpty(yAxisTitle)) plot.Axes.Left.Label.Text = yAxisTitle;
        });
    }
}
