using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class ChartTagHelper : TagHelper
{
	public required List<double> Data { get; set; } = [];

	public bool FillArea { get; set; } = false;
	public string LineColor { get; set; } = "#3b82f6";
	public string FillColor { get; set; } = "#dbeafe";

	public double Width { get; set; } = 500;
	public double Height { get; set; } = 200;
	public double Padding { get; set; } = 20;

	private static string I(FormattableString fs) => FormattableString.Invariant(fs);

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Data.Count == 0)
        {
            output.SuppressOutput();
            return;
        }

        output.TagName = "svg";
        output.Attributes.SetAttribute("viewBox", $"0 0 {Width} {Height}");
        output.Attributes.SetAttribute("width", Width);
        output.Attributes.SetAttribute("height", Height);

        var maxValue = Data.Max();
        var minValue = Math.Min(0, Data.Min());
        var valueRange = maxValue - minValue;
        if (valueRange == 0)
        {
	        valueRange = 1;
        }

        var totalPoints = Data.Count;
        var chartWidth = Width - Padding * 2;
        var chartHeight = Height - Padding * 2;
        var stepX = totalPoints > 1 ? chartWidth / (totalPoints - 1) : chartWidth;

        var points = new List<(double X, double Y)>();
        var index = 0;
        foreach (var value in Data)
        {
            var x = Padding + index * stepX;
            var y = Padding + chartHeight - (value - minValue) / valueRange * chartHeight;
            points.Add((x, y));
            index++;
        }

        var sb = new StringBuilder();

        if (FillArea && points.Count > 1)
        {
            var baselineY = Padding + chartHeight;
            var fillPath = new StringBuilder();
            fillPath.Append(I($"M {points[0].X} {baselineY} "));
            foreach (var p in points)
            {
	            fillPath.Append(I($"L {p.X} {p.Y} "));
            }
            fillPath.Append(I($"L {points[^1].X} {baselineY} "));
            fillPath.Append('Z');

            sb.AppendLine(I($"<path d='{fillPath}' fill='{FillColor}' stroke='none' />"));
        }

        var linePath = new StringBuilder();
        linePath.Append(I($"M {points[0].X} {points[0].Y} "));
        for (var i = 1; i < points.Count; i++)
        {
	        linePath.Append(I($"L {points[i].X} {points[i].Y} "));
        }

        sb.AppendLine(I($"<path d='{linePath}' fill='none' stroke='{LineColor}' stroke-width='3' stroke-linecap='round' stroke-linejoin='round' />"));

        foreach (var p in points)
        {
	        sb.AppendLine(I($"<circle cx='{p.X}' cy='{p.Y}' r='4' fill='{LineColor}' />"));
        }

        output.Content.SetHtmlContent(sb.ToString());
    }
}