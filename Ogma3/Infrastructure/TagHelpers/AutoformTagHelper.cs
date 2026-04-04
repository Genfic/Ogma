using System.Collections;
using System.Collections.Concurrent;
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
				var type = Type.GetTypeCode(prop.PropertyType) switch
				{
					TypeCode.Int16 => "number",
					TypeCode.Int32 => "number",
					TypeCode.Int64 => "number",
					TypeCode.UInt16 => "number",
					TypeCode.UInt32 => "number",
					TypeCode.UInt64 => "number",
					TypeCode.Double => "number",
					TypeCode.Decimal => "number",
					TypeCode.Boolean => "checkbox",
					TypeCode.DateTime => "date",
					_ => "text",
				};
				var label = prop.Name;
				var value = prop.GetValue(obj);

				var hasDatalist = Datalists.TryGetValue(prop.Name, out var datalist);
				var datalistAttr = hasDatalist ? $"""
				                                  list="datalist-{prop.Name.ToLowerInvariant()}"
				                                  """ : "";

				output.Content.AppendHtml("""<div class="o-form-group">""");
				output.Content.AppendHtml($"""<label for="{objName}_{label}">{label.Humanize()}</label>""");
				output.Content.AppendHtml(
					$"""<input {datalistAttr} type="{type}" id="{objName}_{label}" name="{objName}.{label}" value="{value}" class="o-form-control active-border">""");
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
}