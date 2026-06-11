using System.ComponentModel;
using ModelContextProtocol.Server;
using ScottPlot;

namespace GenerateChartMcpServer;

internal partial class ChartTools
{
    [McpServerTool]
    [Description("生成饼图或环形图。labels 为扇区标签 JSON 数组，values 为数值 JSON 数组。返回签名图片 URL。")]
    public string GeneratePieChart(
        [Description("图表标题")] string title,
        [Description("扇区标签，JSON 字符串数组")] string labels,
        [Description("扇区数值，JSON 数字数组")] string values,
        [Description("是否环形图")] bool donut = false,
        [Description("是否爆炸分离")] bool explode = false,
        [Description("切片标签显示百分比")] bool showPercent = false,
        [Description("切片标签显示数值")] bool showValues = false,
        [Description("图例显示百分比")] bool legendPercent = false,
        [Description("图片宽度")] int width = 700,
        [Description("图片高度")] int height = 700)
    {
        var lbls = ParseStrArr(labels);
        var vals = ParseDblArr(values);
        return SaveAndSignUrl(width, height, plot =>
        {
            plot.Title(title);
            var pie = plot.Add.Pie(vals);
            if (donut) pie.DonutFraction = 0.4;
            if (explode) { pie.ExplodeFraction = .1; pie.SliceLabelDistance = 0.5; }
            double total = vals.Sum();
            for (int i = 0; i < lbls.Length; i++)
            {
                var pct = vals[i] / total * 100;
                pie.Slices[i].Label = showPercent ? $"{pct:F1}%" : showValues ? vals[i].ToString() : lbls[i];
                pie.Slices[i].LegendText = legendPercent ? $"{lbls[i]} ({pct:F1}%)" : lbls[i];
            }
            if (pie.Slices.Any(s => s.LegendText != null)) plot.ShowLegend();
            plot.Axes.Frameless(); plot.HideGrid();
        });
    }
}
