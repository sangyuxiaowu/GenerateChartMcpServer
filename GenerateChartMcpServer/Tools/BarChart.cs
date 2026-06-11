using System.ComponentModel;
using ModelContextProtocol.Server;
using ScottPlot;

namespace GenerateChartMcpServer;

internal partial class ChartTools
{
    [McpServerTool]
    [Description("生成柱状图。xLabels 为 X 轴标签 JSON 数组，seriesData 格式：[{\"name\":\"系列名\",\"values\":[1,2,3]}]。返回签名图片 URL。")]
    public string GenerateBarChart(
        [Description("图表标题")] string title,
        [Description("X 轴标签，JSON 字符串数组")] string xLabels,
        [Description("系列数据，JSON 数组")] string seriesData,
        [Description("X 轴标题")] string? xAxisTitle = null,
        [Description("Y 轴标题")] string? yAxisTitle = null,
        [Description("图片宽度")] int width = 800,
        [Description("图片高度")] int height = 600,
        [Description("是否显示数值标签")] bool showValues = false)
    {
        var labels = ParseStrArr(xLabels);
        var series = ParseSeriesData(seriesData);
        return SaveAndSignUrl(width, height, plot =>
        {
            plot.Title(title);
            var colors = GenColors(series.Length);
            var centerPos = Enumerable.Range(1, labels.Length).Select(x => (double)x).ToArray();
            double barWidth = 0.8 / series.Length;
            for (int i = 0; i < series.Length; i++)
            {
                var offset = (i - (series.Length - 1) / 2.0) * barWidth;
                var pos = centerPos.Select(x => x + offset).ToArray();
                var bp = plot.Add.Bars(pos, series[i].Values);
                bp.Color = colors[i]; bp.LegendText = series[i].Name;
                foreach (var b in bp.Bars) b.Size = barWidth;
                if (showValues)
                {
                    foreach (var b in bp.Bars) b.Label = b.Value.ToString();
                    bp.ValueLabelStyle.Bold = true;
                    bp.ValueLabelStyle.FontSize = 10;
                }
            }
            plot.ShowLegend();
            plot.Axes.Bottom.SetTicks(centerPos, labels);
            if (!string.IsNullOrEmpty(xAxisTitle)) plot.Axes.Bottom.Label.Text = xAxisTitle;
            if (!string.IsNullOrEmpty(yAxisTitle)) plot.Axes.Left.Label.Text = yAxisTitle;
            if (showValues) plot.Axes.Margins(bottom: 0, top: .15);
        });
    }

    [McpServerTool]
    [Description("生成条形图（水平）。xLabels 为类别标签 JSON 数组，seriesData 格式同柱状图。返回签名图片 URL。")]
    public string GenerateHorizontalBarChart(
        [Description("图表标题")] string title,
        [Description("类别标签，JSON 字符串数组")] string xLabels,
        [Description("系列数据，JSON 数组")] string seriesData,
        [Description("X 轴标题")] string? xAxisTitle = null,
        [Description("Y 轴标题")] string? yAxisTitle = null,
        [Description("图片宽度")] int width = 800,
        [Description("图片高度")] int height = 600,
        [Description("是否显示数值标签")] bool showValues = false)
    {
        var labels = ParseStrArr(xLabels);
        var series = ParseSeriesData(seriesData);
        return SaveAndSignUrl(width, height, plot =>
        {
            plot.Title(title);
            var colors = GenColors(series.Length);
            var centerPos = Enumerable.Range(1, labels.Length).Select(x => (double)x).ToArray();
            double barWidth = 0.8 / series.Length;
            for (int i = 0; i < series.Length; i++)
            {
                var offset = (i - (series.Length - 1) / 2.0) * barWidth;
                var pos = centerPos.Select(x => x + offset).ToArray();
                var bp = plot.Add.Bars(pos, series[i].Values);
                bp.Color = colors[i]; bp.Horizontal = true; bp.LegendText = series[i].Name;
                foreach (var b in bp.Bars) b.Size = barWidth;
                if (showValues)
                {
                    foreach (var b in bp.Bars) b.Label = b.Value.ToString();
                    bp.ValueLabelStyle.Bold = true;
                    bp.ValueLabelStyle.FontSize = 10;
                }
            }
            plot.ShowLegend();
            plot.Axes.Left.SetTicks(centerPos, labels);
            if (!string.IsNullOrEmpty(xAxisTitle)) plot.Axes.Bottom.Label.Text = xAxisTitle;
            if (!string.IsNullOrEmpty(yAxisTitle)) plot.Axes.Left.Label.Text = yAxisTitle;
        });
    }
}
