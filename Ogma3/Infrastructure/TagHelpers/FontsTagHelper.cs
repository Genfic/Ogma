using System.Text;
using ConfigBinder.Attributes;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Logging.OperationTiming;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class FontsTagHelper(
	IWebHostEnvironment env,
	ILogger<FontsTagHelper> logger,
	IOptions<FontsTagHelper.FontConfig> options
) : TagHelper
{
	private readonly FontEntry[] _fonts  = options.Value.Fonts;

	private readonly HashSet<string> _allowedExtensions  = options.Value.AllowedExtensions;

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
		foreach (var fontEntry in _fonts)
		{
			var fontName = Path.GetFileNameWithoutExtension(fontEntry.Filename).ToLowerInvariant();
			var family = fontEntry.Name ?? fontName;

			if (fonts.TryGetValue(fontName, out var font))
			{
				foreach (var ext in font.Extensions)
				{
					if (_allowedExtensions.Count > 0 && !_allowedExtensions.Contains(ext.Trim('.')))
					{
						continue;
					}
					sb.Append(MakePreloadLink(fontEntry.Filename, ext));
					sb.Append('\n');
					sb.Append(MakeStyle(family, fontEntry.Filename, fontEntry.CssVariableName, ext, fontEntry.Weight, fontEntry.Italic ?? false, fontEntry.Variable ?? true));
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

	private static string MakeStyle(string name, string filename, string cssVariableName, string extension, string? weight, bool italic, bool variable)
	{
		var ext = extension.Trim('.');
		var format = variable ? $"{ext}-variations" : ext;

		var css = //lang=css
			$$"""
			@font-face {
				font-family: "{{name}}";
				font-weight: {{weight ?? "400"}};
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

	[UsedImplicitly]
	public sealed class FontEntry
	{
		public required string Filename { get; init; }
		public required string CssVariableName { get; init; }
		public string? Name { get; init; }
		public string? Weight { get; init; }
		public bool? Italic { get; init; }
		public bool? Variable { get; init; }
	}

	[ConfigSection("Fonts")]
	public sealed class FontConfig
	{
		public required HashSet<string> AllowedExtensions { get; init; }
		public required FontEntry[] Fonts { get; init; }
	}
}