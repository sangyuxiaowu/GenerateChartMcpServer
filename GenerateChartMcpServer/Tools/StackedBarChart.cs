using System.ComponentModel;
using ModelContextProtocol.Server;
using ScottPlot;

namespace GenerateChartMcpServer;

internal partial class ChartTools
{
    [McpServerTool]
    [Description("生成堆叠条形图。xLabels 为类别标签 JSON 数组，seriesData 格式同柱状图。返回签名图片 URL。")]
    public string GenerateStackedBarChart(
        [Description("图表标题")] string title,
        [Description("类别标签，JSON 字符串数组")] string xLabels,
        [Description("系列数据，JSON 数组")] string seriesData,
        [Description("X 轴标题")] string? xAxisTitle = null,
        [Description("Y 轴标题")] string? yAxisTitle = null,
        [Description("图片宽度")] int width = 800,
        [Description("图片高度")] int height = 600,
        [Description("是否显示每个部分的数值标签")] bool showValues = false,
        [Description("显示顶部合计(与showValues互斥)")] bool showTotal = false)
    {
        var labels = ParseStrArr(xLabels);
        var series = ParseSeriesData(seriesData);
        return SaveAndSignUrl(width, height, plot =>
        {
            plot.Title(title);
            var colors = GenColors(series.Length);
            for (int g = 0; g < labels.Length; g++)
            {
                var bars = new List<ScottPlot.Bar>();
                double baseVal = 0, total = 0;
                for (int s = 0; s < series.Length; s++)
                {
                    bars.Add(new ScottPlot.Bar
                    {
                        Position = g,
                        Value = baseVal + series[s].Values[g],
                        ValueBase = baseVal,
                        FillColor = colors[s],
                        Label = (showValues && !showTotal) ? series[s].Values[g].ToString() : string.Empty,
                        CenterLabel = (showValues && !showTotal)
                    });
                    baseVal += series[s].Values[g];
                    total += series[s].Values[g];
                }
                if (showTotal)
                {
                    bars.Last().Label = total.ToString();
                    bars.Last().CenterLabel = false;
                }
                plot.Add.Bars(bars);
            }
            plot.Legend.IsVisible = true;
            for (int s = 0; s < series.Length; s++)
                plot.Legend.ManualItems.Add(new LegendItem { LabelText = series[s].Name, FillColor = colors[s] });
            var tk = new ScottPlot.TickGenerators.NumericManual();
            for (int g = 0; g < labels.Length; g++) tk.AddMajor(g, labels[g]);
            plot.Axes.Bottom.TickGenerator = tk;
            plot.Axes.Margins(bottom: 0, top: (showValues || showTotal) ? .15 : 0);
            if (!string.IsNullOrEmpty(xAxisTitle)) plot.Axes.Bottom.Label.Text = xAxisTitle;
            if (!string.IsNullOrEmpty(yAxisTitle)) plot.Axes.Left.Label.Text = yAxisTitle;
        });
    }
}
