using System.Text;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Infrastructure.Logging.OperationTiming;

namespace Ogma3.Infrastructure.TagHelpers;

using WeightTuple = (int Min, int Max);

public sealed class FontsTagHelper(IWebHostEnvironment env, ILogger<FontsTagHelper> logger) : TagHelper
{
	public required FontEntry[] Fonts { get; set; }

	[UsedImplicitly]
	public required HashSet<string> AllowedExtensions { get; set; } = [];

	private readonly string _fontsPath = Path.Combine(env.WebRootPath, "fonts");

	private static string? _cache;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = null;

		if (_cache is null)
		{
			using var op = logger.TimeOperation("Cache miss. Getting existing fonts...");
			Interlocked.Exchange(ref _cache, GetExistingFonts());
		}

		output.Content.AppendHtml(_cache);

	}

	private string GetExistingFonts()
	{
		var fonts = Directory.EnumerateFiles(_fontsPath, "*", SearchOption.TopDirectoryOnly)
			.GroupBy(f => Path.GetFileNameWithoutExtension(f))
			.ToDictionary(
				g => g.Key,
				g => new { Name = g.Key, Extensions = g.Select(Path.GetExtension).OfType<string>() },
				StringComparer.OrdinalIgnoreCase
			);

		var sb = new StringBuilder();
		foreach (var fontEntry in Fonts)
		{
			var fontName = Path.GetFileNameWithoutExtension(fontEntry.Filename).ToLowerInvariant();
			var family = fontEntry.Name ?? fontName;

			if (fonts.TryGetValue(fontName, out var font))
			{
				foreach (var ext in font.Extensions)
				{
					if (AllowedExtensions.Count > 0 && !AllowedExtensions.Contains(ext.Trim('.')))
					{
						continue;
					}
					sb.Append(MakePreloadLink(fontEntry.Filename, ext));
					sb.Append('\n');
					sb.Append(MakeStyle(family, fontEntry.Filename, fontEntry.CssVariableName, ext, fontEntry.Weight ?? 400, fontEntry.Italic, fontEntry.Variable));
				}
				continue;
			}

			logger.LogWarning("Font {Name} was not found in {Path}.", fontName, _fontsPath);
		}

		return sb.ToString();
	}

	private static string MakePreloadLink(string filename, string extension)
	{
		var ext = extension.Trim('.');

		return //lang=html
			$"""
	        <link rel="preload" href="/fonts/{filename}.{ext}" as="font" type="font/{ext}" crossorigin="anonymous">
	        """;
	}

	private static string MakeStyle(string name, string filename, string cssVariableName, string extension, FontWeight weight, bool italic, bool variable)
	{
		var ext = extension.Trim('.');
		var weightString = weight switch
		{
			int i => i.ToString(),
			WeightTuple t => $"{t.Min} {t.Max}",
		};
		var format = variable ? $"{ext}-variations" : ext;

		var css = //lang=css
			$$"""
			@font-face {
				font-family: "{{name}}";
				font-weight: {{weightString}};
				font-style: {{(italic ? "italic" : "normal")}};
				font-display: swap;
				src:
					local("{{name}}"),
					local("{{filename}}"),
					url("/fonts/{{filename}}.{{ext}}") format("{{format}}");
			}

			:root {
				--font-{{cssVariableName}}: "{{name}}";
			}
			""";

		return // lang=html
			$"""
			 <style>
			 {css}
			 </style>
			 """;
	}

	public sealed record FontEntry(
		string Filename,
		string CssVariableName,
		string? Name = null,
		FontWeight? Weight = null,
		bool Italic = false,
		bool Variable = true
	);

	public union FontWeight(int, WeightTuple);
}