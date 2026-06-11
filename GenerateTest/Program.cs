using ScottPlot;
using SkiaSharp;

string fontName = SKFontManager.Default.MatchCharacter('汉').FamilyName;

var outDir = @"e:\work\GenerateChartMcpServer\GenerateTest\output";
Directory.CreateDirectory(outDir);
int ok = 0, fail = 0;
void Save(Plot p, string n, int w, int h) { var path = Path.Combine(outDir, n); try { p.SavePng(path, w, h); Console.WriteLine($"  [OK] {n}  ({new FileInfo(path).Length/1024} KB)"); ok++; } catch (Exception ex) { Console.WriteLine($"  [FAIL] {n}: {ex.Message}"); fail++; } }

Console.WriteLine("\n=== ScottPlot Test ===\n");

// 1. 柱状图 + 数值标签
Console.WriteLine("[1] 柱状图");
var p1 = new Plot(); p1.Font.Set(fontName); p1.Title("柱状图 - 各季度销售额");
string[] labs = {"Q1","Q2","Q3","Q4"};
double[][] vals = {new double[]{120,200,150,180}, new double[]{90,160,200,140}};
Color[] clrs = {Colors.SlateBlue, Colors.Orange};
double bw = 0.8 / vals.Length;
for(int i=0;i<vals.Length;i++){
    var pos = Enumerable.Range(1,4).Select(x=>x+(i-(vals.Length-1)/2.0)*bw).ToArray();
    var bp = p1.Add.Bars(pos, vals[i]); bp.Color=clrs[i]; bp.LegendText=new[]{"产品A","产品B"}[i];
    foreach(var b in bp.Bars){ b.Size = bw; b.Label = b.Value.ToString(); }
    bp.ValueLabelStyle.Bold = true; bp.ValueLabelStyle.FontSize = 10;
}
p1.ShowLegend(); p1.Axes.Bottom.SetTicks(new double[]{1,2,3,4}, labs);
p1.Axes.Bottom.Label.Text="季度"; p1.Axes.Left.Label.Text="万元";
p1.Axes.Margins(bottom:0, top:.15);
Save(p1, "01_bar.png", 800, 600);

// 2a. 条形图 - 单
Console.WriteLine("[2a] 条形图(单)");
var p2a = new Plot(); p2a.Font.Set(fontName); p2a.Title("条形图(单) - 产品销售额");
var hb1 = p2a.Add.Bars(new double[]{120,200,150,180}); hb1.Color=Colors.MediumSeaGreen; hb1.Horizontal=true; hb1.LegendText="销售额";
foreach(var b in hb1.Bars) b.Label = b.Value.ToString();
hb1.ValueLabelStyle.Bold = true; hb1.ValueLabelStyle.FontSize = 10;
p2a.ShowLegend(); p2a.Axes.Left.SetTicks(new double[]{1,2,3,4}, new[]{"产品A","产品B","产品C","产品D"});
Save(p2a, "02a_hbar_single.png", 800, 600);

// 2b. 条形图 - 双
Console.WriteLine("[2b] 条形图(双)");
var p2b = new Plot(); p2b.Font.Set(fontName); p2b.Title("条形图(双) - 各季度对比");
double[][] v2b = {new double[]{120,200,150,180}, new double[]{90,160,200,140}};
Color[] c2b = {Colors.MediumSeaGreen, Colors.Orange};
double bw2b = 0.8 / v2b.Length;
for(int i=0;i<v2b.Length;i++){
    var pos = Enumerable.Range(1,4).Select(x=>x+(i-(v2b.Length-1)/2.0)*bw2b).ToArray();
    var bp = p2b.Add.Bars(pos, v2b[i]); bp.Color=c2b[i]; bp.Horizontal=true; bp.LegendText=new[]{"产品A","产品B"}[i];
    foreach(var b in bp.Bars){ b.Size = bw2b; b.Label = b.Value.ToString(); }
    bp.ValueLabelStyle.Bold = true; bp.ValueLabelStyle.FontSize = 10;
}
p2b.ShowLegend(); p2b.Axes.Left.SetTicks(new double[]{1,2,3,4}, new[]{"Q1","Q2","Q3","Q4"});
Save(p2b, "02b_hbar_double.png", 800, 600);

// 2c. 条形图 - 三
Console.WriteLine("[2c] 条形图(三)");
var p2c = new Plot(); p2c.Font.Set(fontName); p2c.Title("条形图(三) - 各季度对比");
double[][] v2c = {new double[]{120,200,150,180}, new double[]{90,160,200,140}, new double[]{70,100,120,90}};
Color[] c2c = {Colors.MediumSeaGreen, Colors.Orange, Colors.SlateBlue};
double bw2c = 0.8 / v2c.Length;
for(int i=0;i<v2c.Length;i++){
    var pos = Enumerable.Range(1,4).Select(x=>x+(i-(v2c.Length-1)/2.0)*bw2c).ToArray();
    var bp = p2c.Add.Bars(pos, v2c[i]); bp.Color=c2c[i]; bp.Horizontal=true; bp.LegendText=new[]{"产品A","产品B","产品C"}[i];
    foreach(var b in bp.Bars){ b.Size = bw2c; b.Label = b.Value.ToString(); }
    bp.ValueLabelStyle.Bold = true; bp.ValueLabelStyle.FontSize = 9;
}
p2c.ShowLegend(); p2c.Axes.Left.SetTicks(new double[]{1,2,3,4}, new[]{"Q1","Q2","Q3","Q4"});
Save(p2c, "02c_hbar_triple.png", 800, 600);

// 3. 堆叠条形图 (CenterLabel)
Console.WriteLine("[3] 堆叠条形图");
var p3 = new Plot(); p3.Font.Set(fontName); p3.Title("堆叠条形图");
string[] sg = {"Q1","Q2","Q3","Q4"};
double[][] sv = {new double[]{10,20,30,40}, new double[]{15,25,35,45}};
Color[] sc = {Colors.SlateBlue, Colors.Orange};
for(int g=0; g<sg.Length; g++){
    var bars = new List<ScottPlot.Bar>();
    double baseVal = 0;
    for(int s=0; s<sv.Length; s++){
        bars.Add(new ScottPlot.Bar{Position=g, Value=baseVal+sv[s][g], ValueBase=baseVal, FillColor=sc[s], Label=sv[s][g].ToString(), CenterLabel=true});
        baseVal += sv[s][g];
    }
    p3.Add.Bars(bars);
}
p3.Legend.IsVisible=true;
for(int s=0;s<2;s++) p3.Legend.ManualItems.Add(new LegendItem{LabelText=new[]{"产品A","产品B"}[s], FillColor=sc[s]});
var tk = new ScottPlot.TickGenerators.NumericManual();
for(int g=0;g<4;g++) tk.AddMajor(g, sg[g]);
p3.Axes.Bottom.TickGenerator=tk; p3.Axes.Margins(bottom:0, top:.15);
Save(p3, "03_stacked_bar.png", 800, 600);

// 3b. 堆叠条形图 - 顶部显示合计
Console.WriteLine("[3b] 堆叠条形图(顶部合计)");
var p3b = new Plot(); p3b.Font.Set(fontName); p3b.Title("堆叠条形图 - 顶部显示合计");
for(int g=0; g<sg.Length; g++){
    var bars = new List<ScottPlot.Bar>();
    double baseVal = 0, total = 0;
    for(int s=0; s<sv.Length; s++){
        bars.Add(new ScottPlot.Bar{Position=g, Value=baseVal+sv[s][g], ValueBase=baseVal, FillColor=sc[s], Label=sv[s][g].ToString(), CenterLabel=true});
        baseVal += sv[s][g]; total += sv[s][g];
    }
    bars.Last().Label = total.ToString();
    bars.Last().CenterLabel = false;
    p3b.Add.Bars(bars);
}
p3b.Legend.IsVisible=true;
for(int s=0;s<2;s++) p3b.Legend.ManualItems.Add(new LegendItem{LabelText=new[]{"产品A","产品B"}[s], FillColor=sc[s]});
var tk3b = new ScottPlot.TickGenerators.NumericManual();
for(int g=0;g<4;g++) tk3b.AddMajor(g, sg[g]);
p3b.Axes.Bottom.TickGenerator=tk3b; p3b.Axes.Margins(bottom:0, top:.15);
Save(p3b, "03b_stacked_total.png", 800, 600);

// 4. 折线图
Console.WriteLine("[4] 折线图");
var p4 = new Plot(); p4.Font.Set(fontName); p4.Title("折线图 - 月度用户增长");
var xv = new double[]{1,2,3,4,5,6}; var xl = new[]{"1月","2月","3月","4月","5月","6月"};
var la = p4.Add.Scatter(xv, new double[]{1200,1500,1800,2100,2500,2800}); la.Color=Colors.RoyalBlue; la.LegendText="新增用户"; la.LineWidth=2; la.MarkerSize=8;
var lb = p4.Add.Scatter(xv, new double[]{5000,5300,5600,6000,6500,7000}); lb.Color=Colors.OrangeRed; lb.LegendText="活跃用户"; lb.LineWidth=2; lb.MarkerSize=8;
p4.ShowLegend(); p4.Axes.Bottom.SetTicks(xv, xl); p4.Axes.Bottom.Label.Text="月份"; p4.Axes.Left.Label.Text="用户数";
Save(p4, "04_line.png", 800, 600);

// 4b. 折线图 + Tooltip标注
Console.WriteLine("[4b] 折线图(Tooltip)");
xv = xv.Append(7).ToArray(); xl = xl.Append("7月").ToArray();
var p4b = new Plot(); p4b.Font.Set(fontName); p4b.Title("折线图 - Tooltip标注");
var la2 = p4b.Add.Scatter(xv, new double[]{1200,1500,1800,2100,2500,2800,2000}); la2.Color=Colors.RoyalBlue; la2.LegendText="新增用户"; la2.LineWidth=2; la2.MarkerSize=8;
// Add tooltip on peak point
var tip = new ScottPlot.Coordinates(6, 2800);
var label = tip.WithDelta(1, .7);
p4b.Add.Tooltip(tip, "峰值: 2800", label);
// Add tooltip on another point
p4b.Add.Tooltip(new ScottPlot.Coordinates(1, 1200), "起点: 1200", new ScottPlot.Coordinates(1, 1400));
p4b.ShowLegend(); p4b.Axes.Bottom.SetTicks(xv, xl); p4b.Axes.Bottom.Label.Text="月份"; p4b.Axes.Left.Label.Text="用户数";
Save(p4b, "04b_line_tooltip.png", 800, 600);

// 5. 饼图
Console.WriteLine("[5] 饼图");
var p5 = new Plot(); p5.Font.Set(fontName); p5.Title("饼图 - 市场份额");
var pie5 = p5.Add.Pie(new double[]{35,28,22,10,5});
for(int i=0;i<5;i++) pie5.Slices[i].Label = new[]{"苹果","三星","华为","小米","其他"}[i];
p5.Axes.Frameless(); p5.HideGrid();
Save(p5, "05_pie.png", 700, 700);

// 6. 环形图
Console.WriteLine("[6] 环形图");
var p6 = new Plot(); p6.Font.Set(fontName); p6.Title("环形图 - 任务进度");
var pie6 = p6.Add.Pie(new double[]{60,25,15}); pie6.DonutFraction=0.4;
for(int i=0;i<3;i++) pie6.Slices[i].Label = new[]{"已完成","进行中","未开始"}[i];
p6.Axes.Frameless(); p6.HideGrid();
Save(p6, "06_doughnut.png", 700, 700);

// 6b. 环形图 - 爆炸 + 图例
Console.WriteLine("[6b] 环形图(爆炸+图例)");
var p6b = new Plot(); p6b.Font.Set(fontName); p6b.Title("环形图 - 爆炸 + 自定义图例");
var slices6b = new List<ScottPlot.PieSlice>{
    new(){Value=5, FillColor=Colors.Red, Label="Red", LegendText="R"},
    new(){Value=2, FillColor=Colors.Orange, Label="Orange"},
    new(){Value=8, FillColor=Colors.Gold, Label="Yellow"},
    new(){Value=4, FillColor=Colors.Green, Label="Green", LegendText="G"},
    new(){Value=8, FillColor=Colors.Blue, Label="Blue", LegendText="B"},
};
var pie6b = p6b.Add.Pie(slices6b); pie6b.ExplodeFraction=.1; pie6b.SliceLabelDistance=1.4; pie6b.DonutFraction=0.3;
p6b.ShowLegend(); p6b.Axes.Frameless(); p6b.HideGrid();
Save(p6b, "06b_doughnut_explode.png", 700, 700);

// 6c. 饼图 - 显示百分比
Console.WriteLine("[6c] 饼图(百分比)");
var p6c = new Plot(); p6c.Font.Set(fontName); p6c.Title("饼图 - 百分比");
double[] vals6c = {6, 8, 10};
var pie6c = p6c.Add.Pie(vals6c); pie6c.ExplodeFraction=.1; pie6c.SliceLabelDistance=0.5;
double total6c = vals6c.Sum();
var pcts6c = vals6c.Select(v => v/total6c*100).ToArray();
for(int i=0;i<3;i++) pie6c.Slices[i].Label = $"{pcts6c[i]:F1}%";
p6c.Axes.Frameless(); p6c.HideGrid();
Save(p6c, "06c_pie_percent.png", 700, 700);
// 6d. 饼图 - 图例显示百分比
Console.WriteLine("[6d] 饼图(图例百分比)");
var p6d = new Plot(); p6d.Font.Set(fontName); p6d.Title("饼图 - 图例含百分比");
string[] names6d = {"苹果","三星","华为","小米","其他"};
double[] vals6d = {35,28,22,10,5};
var pie6d = p6d.Add.Pie(vals6d); pie6d.ExplodeFraction=.05;
double total6d = vals6d.Sum();
for(int i=0;i<5;i++){
    var pct = vals6d[i]/total6d*100;
    pie6d.Slices[i].Label = names6d[i];
    pie6d.Slices[i].LegendText = $"{names6d[i]} ({pct:F1}%)";
}
p6d.ShowLegend(); p6d.Axes.Frameless(); p6d.HideGrid();
Save(p6d, "06d_pie_legend_pct.png", 700, 700);
// 7. 气泡图
Console.WriteLine("[7] 气泡图");
var p7 = new Plot(); p7.Font.Set(fontName); p7.Title("气泡图");
var bua = p7.Add.Scatter(new double[]{1,2,3,4}, new double[]{10,20,30,40}); bua.Color=Colors.RoyalBlue.WithAlpha(0.5); bua.LegendText="城市A"; bua.MarkerSize=20; bua.LineWidth=0;
var bub = p7.Add.Scatter(new double[]{1.5,2.5,3.5,4.5}, new double[]{15,25,35,45}); bub.Color=Colors.OrangeRed.WithAlpha(0.5); bub.LegendText="城市B"; bub.MarkerSize=28; bub.LineWidth=0;
p7.ShowLegend();
Save(p7, "07_bubble.png", 800, 600);

Console.WriteLine($"\n=== {ok} OK, {fail} FAIL ===");
