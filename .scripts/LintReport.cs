#!usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

const int contextLines = 3;

var solutionDir = Path.GetFullPath("..");
var frontendDir = Path.Join(solutionDir, "FrontendCode");
var binPath = Path.Join(frontendDir, "node_modules", ".bin", "oxlint.exe");

var startInfo = new ProcessStartInfo
{
	FileName = binPath,
	WorkingDirectory = frontendDir,
	ArgumentList = { ".", "--format=json" },
	UseShellExecute = false,
	RedirectStandardOutput = true,
	RedirectStandardError = true,
};

var proc = await Process.RunAndCaptureTextAsync(startInfo);
var data = JsonSerializer.Deserialize(proc.StandardOutput, OxlintJsonContext.Default.OxlintReport);

if (data is null)
{
	Console.Error.WriteLine("Could not parse JSON");
	return 1;
}

Console.WriteLine($"Found {data.Diagnostics.Length} diagnostics");

var app = WebApplication.Create();
app.MapGet("/", async () => {
	var vm = await BuildVm(data);
	return TypedResults.Content(Web.Html(vm), "text/html; charset=utf-8");
});
app.Run();

return 0;

static async Task<ViewModel> BuildVm(OxlintReport report)
{
	Console.WriteLine($"Linting {report.NumberOfFiles} files");

	var tasks = report.Diagnostics.Select(async diag => {
		var primary = diag.Labels.FirstOrDefault()?.Span;
		var snippets = primary is not null
			? await GetSnippet(diag.Path, primary)
			: [];

		var ruleId = Helpers.EslintRegex.Replace(diag.Code, "");
		var lineInfo = primary is not null ? $"{diag.Path}:{primary.Line}:{primary.Column}" : diag.Path;
		return new ExtendedDiagnostic(diag, snippets, primary is not null, ruleId, lineInfo);
	});

	var diagnostics = await Task.WhenAll(tasks);
	var counts = diagnostics
		.GroupBy(d => d.Diagnostic.Severity)
		.ToDictionary(g => g.Key.ToLowerInvariant(), g => g.Count());

	var summary = new Summary(
		Errors: counts.TryGetValue("error", out var errors) ? errors : 0,
		Warnings: counts.TryGetValue("warning", out var warnings) ? warnings : 0,
		Info: counts.TryGetValue("info", out var info) ? info : 0,
		Total: diagnostics.Length,
		Linted: report.NumberOfFiles,
		RulesRan: report.NumberOfRules,
		Duration: report.StartTime * 1000
	);

	return new ViewModel(diagnostics, summary);
}

static async Task<List<Snippet>> GetSnippet(string path, Span span)
{
	var lines = await File.ReadAllLinesAsync(path);

	Console.WriteLine($"Read {lines.Length} lines from {path}");

	var target = span.Line;
	var start = Math.Max(1, target - contextLines);
	var end = Math.Min(lines.Length, target + contextLines);

	var snippets = new List<Snippet>(lines.Length);
	for (var ln = start; ln <= end; ln++)
	{
		var text = lines.ElementAtOrDefault(ln - 1) ?? "";
		var isTarget = ln == target;


		if (!isTarget)
		{
			snippets.Add(new(ln, isTarget, EscapeHtml(text), "", ""));
			continue;
		}

		var hlStart = span.Column - 1;
		var hlEnd = hlStart + span.Length;
		var before = EscapeHtml(text[..hlStart]);
		var highlighted = EscapeHtml(text[hlStart..hlEnd]);
		var after = EscapeHtml(text[hlEnd..]);

		snippets.Add(new(ln, isTarget, before, highlighted, after));
	}

	return snippets;
}

static string EscapeHtml(string str)
	=> str
		.Replace("&", "&amp;")
		.Replace("<", "&lt;")
		.Replace(">", "&gt;")
		.Replace("\"", "&quot;")
		.Replace("'", "&#39;");

internal static partial class Helpers
{
	[GeneratedRegex(@"^eslint\(|\)$")]
	public static partial Regex EslintRegex { get; }
}

internal sealed record Snippet(int Number, bool IsTarget, string Before, string Highlighted, string After);

internal sealed record ExtendedDiagnostic(Diagnostic Diagnostic, List<Snippet> Snippets, bool Missing, string RuleId, string LineInfo);

internal sealed record Summary(int Errors, int Warnings, int Info, int Total, int Linted, int RulesRan, double Duration);

internal sealed record ViewModel(ExtendedDiagnostic[] Diagnostics, Summary Summary);

// Input JSON types
internal sealed record Span(int Offset, int Length, int Line, int Column);

internal sealed record Label(Span Span, [property: JsonPropertyName("label")] string? Name);

internal sealed record Diagnostic
(
	string Message,
	string Code,
	string Severity,
	string[] Causes,
	string Url,
	string Help,
	string Filename,
	Label[] Labels,
	object[] Related)
{
	public string Path { get; } = Filename.Replace("%20", " ").Replace("file:///", "");
}

internal sealed record OxlintReport(Diagnostic[] Diagnostics, int NumberOfFiles, int NumberOfRules, int ThreadsCount, double StartTime);

[JsonSerializable(typeof(OxlintReport))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower, PropertyNameCaseInsensitive = true)]
internal sealed partial class OxlintJsonContext : JsonSerializerContext;

internal static class Web
{
	public static string Html(ViewModel vm) => // lang=html
		$"""
	    <!DOCTYPE html>
	    <html lang="en">
	    <head>
	        <meta charset="UTF-8" />
	        <title>Oxlint Report</title>
	        <style>
	    		{Css}
	        </style>
	    </head>
	    <body>
	    <header>
	        <h1>🦀 Oxlint Report</h1>
	        <div class="summary">
	            <div class="stat errors">
	                <div class="value">{vm.Summary.Errors}</div>
	                <div class="label">Errors</div>
	            </div>
	            <div class="stat warnings">
	                <div class="value">{vm.Summary.Warnings}</div>
	                <div class="label">Warnings</div>
	            </div>
	            <div class="stat info">
	                <div class="value">{vm.Summary.Info}</div>
	                <div class="label">Informations</div>
	            </div>
	            <div class="stat">
	                <div class="value">{vm.Summary.Linted}</div>
	                <div class="label">Files linted</div>
	            </div>
	            <div class="stat">
	                <div class="value">{vm.Summary.RulesRan}</div>
	                <div class="label">Rules run</div>
	            </div>
	            <div class="stat">
	                <div class="value">{vm.Summary.Duration} ms</div>
	                <div class="label">Duration</div>
	            </div>
	        </div>
	    </header>

	    <main>
	    {(
		    vm.Diagnostics.Length == 0
		    ? /* lang=html */ """
			  <div class="empty-state">✅ No issues found. Clean run!</div>
			  """
		    : string.Join("\n", vm.Diagnostics.Select(diag => //lang=html
				$"""
				<div class="diagnostic">
				    <div class="diagnostic-header">
					    <div>
						    <div class="message">
							    <span class="severity-badge {diag.Diagnostic.Severity}">{diag.Diagnostic.Severity}</span>
							    {diag.Diagnostic.Message}
						    </div>
						    <div class="file-location">{diag.LineInfo}</div>
					    </div>
					    <a class="rule-code" href="{diag.Diagnostic.Url}" target="_blank" rel="noopener">
						    {diag.Diagnostic.Code}
					    </a>
				    </div>
				    {(
						diag.Missing
						? // lang=html
						$"""
						<div class="missing-file">Source file not found — cannot render snippet for {diag.Diagnostic.Filename}</div>
						"""
						: // lang=html
						$"""
						<div class="code-block">
						{string.Join("\n", diag.Snippets.Select(line => // lang=html
							$"""
							<div class="code-line {(line.IsTarget ? "target" : "")}">
								<div class="line-number">{line.Number}</div>
								<div class="line-content">
									{line.Before}{(
										line.IsTarget ? // lang=html
											$"""
											<span class="hl">{line.Highlighted}</span>{line.After}
											"""
											: string.Empty
									)}
								</div>
							</div>
							"""))}
						</div>
						"""
				    )}

					{(string.IsNullOrWhiteSpace(diag.Diagnostic.Help) ? string.Empty
						:  //lang=html
						$"""
						<div class="help-box"><strong>💡 Help:</strong> {diag.Diagnostic.Help}</div>
						"""
					)}
				</div>
				"""
		    ))
	    )}
	    </main>
	    </body>
	    </html>
	    """;

	private const string Css = // lang=css
		"""
		:root {
			--bg: #0f1117;
			--panel: #171a23;
			--border: #262b38;
			--text: #e4e6eb;
			--muted: #8a8f9c;
			--error: #f47174;
			--warning: #f5c453;
			--info: #2ae3db;
			--accent: #6ea8fe;
			--code-bg: #0b0d12;
			--highlight: rgba(244, 113, 116, 0.25);
		}
		* { box-sizing: border-box; }
		body {
			margin: 0;
			font-family: "Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
			background: var(--bg);
			color: var(--text);
			line-height: 1.5;
		}
		header {
			padding: 2rem 2.5rem 1.5rem;
			border-bottom: 1px solid var(--border);
		}
		header h1 {
			margin: 0 0 0.5rem;
			font-size: 1.6rem;
			font-weight: 700;
		}
		.summary {
			display: flex;
			gap: 1.5rem;
			flex-wrap: wrap;
			margin-top: 1rem;
		}
		.stat {
			background: var(--panel);
			border: 1px solid var(--border);
			border-radius: 10px;
			padding: 0.75rem 1.25rem;
			min-width: 120px;
		}
		.stat .value {
			font-size: 1.5rem;
			font-weight: 700;
		}
		.stat .label {
			font-size: 0.78rem;
			color: var(--muted);
			text-transform: uppercase;
			letter-spacing: 0.03em;
		}
		.stat.errors .value { color: var(--error); }
		.stat.warnings .value { color: var(--warning); }
		.stat.info .value { color: var(--info); }

		main { padding: 1.5rem 2.5rem 3rem; max-width: 1100px; margin: 0 auto; }

		.empty-state {
			text-align: center;
			color: var(--muted);
			padding: 4rem 0;
			font-size: 1.1rem;
		}

		.diagnostic {
			background: var(--panel);
			border: 1px solid var(--border);
			border-radius: 12px;
			margin-bottom: 1.25rem;
			overflow: hidden;
		}
		.diagnostic-header {
			display: flex;
			align-items: flex-start;
			justify-content: space-between;
			gap: 1rem;
			padding: 1rem 1.25rem;
			border-bottom: 1px solid var(--border);
		}
		.severity-badge {
			display: inline-block;
			font-size: 0.7rem;
			font-weight: 700;
			text-transform: uppercase;
			padding: 0.2rem 0.55rem;
			border-radius: 6px;
			margin-right: 0.6rem;
			vertical-align: middle;
		}
		.severity-badge.error { background: rgba(244,113,116,0.15); color: var(--error); }
		.severity-badge.warning { background: rgba(245,196,83,0.15); color: var(--warning); }

		.message { font-weight: 600; font-size: 1rem; }
		.rule-code {
			font-family: "SF Mono", Consolas, monospace;
			font-size: 0.8rem;
			color: var(--accent);
			text-decoration: none;
		}
		.rule-code:hover { text-decoration: underline; }

		.file-location {
			font-family: "SF Mono", Consolas, monospace;
			font-size: 0.8rem;
			color: var(--muted);
			white-space: nowrap;
			margin-top: 0.35rem;
		}

		.code-block {
			background: var(--code-bg);
			overflow-x: auto;
			font-family: "SF Mono", Consolas, "Courier New", monospace;
			font-size: 0.85rem;
		}
		.code-line {
			display: flex;
		}
		.code-line.target { background: rgba(244,113,116,0.08); }
		.line-number {
			width: 3.5rem;
			flex-shrink: 0;
			text-align: right;
			padding: 0.15rem 0.75rem;
			color: var(--muted);
			user-select: none;
			border-right: 1px solid var(--border);
		}
		.line-content {
			padding: 0.15rem 1rem;
			white-space: pre;
			flex: 1;
		}
		.hl {
			background: var(--highlight);
			border-radius: 3px;
			outline: 1px solid var(--error);
		}

		.help-box {
			padding: 0.85rem 1.25rem;
			font-size: 0.85rem;
			color: var(--muted);
			border-top: 1px solid var(--border);
		}
		.help-box strong { color: var(--text); }
		.missing-file {
			padding: 1rem 1.25rem;
			color: var(--muted);
			font-style: italic;
			font-size: 0.85rem;
		}
		""";
}