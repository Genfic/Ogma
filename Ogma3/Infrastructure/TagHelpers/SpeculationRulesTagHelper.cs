using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NetEscapades.EnumGenerators;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class SpeculationRulesTagHelper : TagHelper
{
	public SpeculationRule[]? Prefetch { get; set; }
	public SpeculationRule[]? Prerender { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var rules = new SpeculationRules(Prefetch, Prerender);

		var json = JsonSerializer.Serialize(rules, SpeculationRulesContext.Default.SpeculationRules);
		output.TagName = "script";
		output.Attributes.Add("type", "speculationrules");
		output.Content.SetHtmlContent(json);
	}
}

public sealed record SpeculationRules(SpeculationRule[]? Prefetch, SpeculationRule[]? Prerender);

[EnumExtensions]
public enum SpeculationSource
{
	Document,
	List,
}

[EnumExtensions]
public enum SpeculationEagerness
{
	Immediate,
	Eager,
	Moderate,
	Conservative,
}

public sealed class SpeculationRule
{
	public required string?[] Urls { get; init; }
	public string[]? Requires { get; init; }
	public SpeculationSource? Source { get; set; }
	public SpeculationEagerness? Eagerness { get; set; }
	public string? ExpectsNoVarySearch { get; set; }
	public string? ReferrerPolicy { get; init; }
}

[JsonSerializable(typeof(SpeculationRules))]
[JsonSourceGenerationOptions(
	WriteIndented = true,
	PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
	UseStringEnumConverter = true
)]
public sealed partial class SpeculationRulesContext : JsonSerializerContext;