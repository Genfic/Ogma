using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Infrastructure.Attributes;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class AutoformTagHelper(IHtmlGenerator generator) : TagHelper
{
	[HtmlAttributeNotBound]
	[ViewContext]
	public required ViewContext ViewContext { get; set; }

	private IHtmlGenerator Generator { get; } = generator;

	public required ModelExpression For { get; set; }

	public string Action { get; set; } = "";

	private static readonly ConcurrentDictionary<Type, Dictionary<string, List<PropertyInfo>>> GroupedPropertiesCache = new();

	[HtmlAttributeName(DictionaryAttributePrefix = "datalist-for-")]
	public IDictionary<string, object?> Datalists { get; set; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "form";
		output.AddClass("form", NullHtmlEncoder.Default);
		output.AddClass("autoform", NullHtmlEncoder.Default);
		output.Attributes.Add("id", "config");
		output.Attributes.Add("method", "post");
		output.Attributes.Add("action", Action);

		var obj = For.Model;
		var objName = For.Name;

		var modelType = obj.GetType();

		var groupedProps = GroupedPropertiesCache.GetOrAdd(
			modelType,
			modelType
				.GetProperties()
				.GroupBy(p => p.GetCustomAttribute<AutoformCategoryAttribute>()?.Name ?? "General")
				.ToDictionary(p => p.Key, p => p.ToList())
		);

		foreach (var (key, propertyInfos) in groupedProps)
		{
			output.Content.AppendHtml("<details>");
			output.Content.AppendHtml($"<summary>{key}</summary>");

			foreach (var prop in propertyInfos)
			{
				var (type, meta) = Type.GetTypeCode(prop.PropertyType) switch
				{
					TypeCode.Int16 => new FieldType("number"),
					TypeCode.Int32 => new FieldType("number"),
					TypeCode.Int64 => new FieldType("number"),
					TypeCode.UInt16 => new FieldType("number"),
					TypeCode.UInt32 => new FieldType("number"),
					TypeCode.UInt64 => new FieldType("number"),
					TypeCode.Double => new FieldType("number", new(){["step"] = "0.01"}),
					TypeCode.Decimal => new FieldType("number", new(){["step"] = "0.01"}),
					TypeCode.Boolean => new FieldType("checkbox"),
					TypeCode.DateTime => new FieldType("date"),
					_ => new FieldType("text"),
				};

				var label = prop.Name;

				var val = prop.GetValue(obj);
				var value = val is IFormattable f
					? f.ToString(null, CultureInfo.InvariantCulture)
					: val?.ToString();

				var hasDatalist = Datalists.TryGetValue(prop.Name, out var datalist);

				if (hasDatalist)
				{
					meta ??= new()
					{
						["list"] = $"list=\"datalist-{prop.Name.ToLowerInvariant()}\"",
					};
				}

				var attrs = meta?
					.Select(kvp => $"{kvp.Key}=\"{kvp.Value}\"")
					.Aggregate((a, b) => $"{a} {b}");

				output.Content.AppendHtml("""<div class="o-form-group">""");
				output.Content.AppendHtml($"""<label for="{objName}_{label}">{label.Humanize()}</label>""");
				output.Content.AppendHtml(
					$"""<input {attrs} type="{type}" id="{objName}_{label}" name="{objName}.{label}" value="{value}" class="o-form-control active-border">""");
				if (hasDatalist && datalist is IEnumerable items)
				{
					output.Content.AppendHtml($"""<datalist id="datalist-{prop.Name.ToLowerInvariant()}">""");
					foreach (var item in items)
					{
						output.Content.AppendHtml($"""<option value="{item}"></option>""");
					}
					output.Content.AppendHtml("</datalist>");
				}
				output.Content.AppendHtml("</div>");
			}

			output.Content.AppendHtml("</details>");
		}

		output.Content.AppendHtml("""<div class="o-form-group">""");
		output.Content.AppendHtml("""<button type="submit" class="btn btn-primary">Save</button>""");
		output.Content.AppendHtml("</div>");

		var xcsrf = Generator.GenerateAntiforgery(ViewContext);
		if (xcsrf is not null)
		{
			output.PostContent.AppendHtml(xcsrf);
		}
	}

	private sealed record FieldType(string Type, Dictionary<string, string>? Meta = null);
}