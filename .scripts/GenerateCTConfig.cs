#!/usr/bin/dotnet run
#:package Microsoft.CodeAnalysis.CSharp@5.*
#:property PublishAot = false
#:property LangVersion = preview
#:property TargetFramework = net11.0

using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Text.Json;

var start = Stopwatch.StartNew();

var sourcePath = args[0];
var targetDir = args[1];

var source = await File.ReadAllTextAsync(sourcePath);
var tree = CSharpSyntaxTree.ParseText(source);
var root = tree.GetCompilationUnitRoot();

var rootClass = root.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()
	.First(c => c.Identifier.Text == "CTConfig");

// Populated as we go so spreads can reference already-parsed top-level arrays
var result = new Dictionary<string, object>();

foreach (var field in rootClass.Members.OfType<FieldDeclarationSyntax>())
{
	foreach (var v in field.Declaration.Variables)
	{
		if (ParseValue(v.Initializer?.Value, result) is not {} parsed)
		{
			continue;
		}
		result[v.Identifier.Text] = parsed;
	}
}

foreach (var nested in rootClass.Members.OfType<ClassDeclarationSyntax>())
{
	var section = new Dictionary<string, object>();
	foreach (var field in nested.Members.OfType<FieldDeclarationSyntax>())
	{
		foreach (var v in field.Declaration.Variables)
		{
			if (ParseValue(v.Initializer?.Value, result) is not {} parsed)
			{
				continue;
			}
			section[v.Identifier.Text] = parsed;
		}
	}

	result[nested.Identifier.Text] = section;
}

var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true, IndentCharacter = '\t', IndentSize = 1 });
await File.WriteAllTextAsync(Path.Combine(targetDir, "ctconfig.json"), json);

var ts = new StringBuilder();
ts.AppendLine("// Auto-generated from CTConfig.cs — do not edit manually");
ts.AppendLine();

foreach (var (key, value) in result)
{
	if (value is Dictionary<string, object> section)
	{
		ts.AppendLine($"export const {key} = {{");
		foreach (var (k, v) in section)
		{
			ts.AppendLine($"\t{k}: {ToTs(v)},");
		}
		ts.AppendLine("} as const;");
		ts.AppendLine();
	}
	else
	{
		ts.AppendLine($"export const {key} = {ToTs(value)};");
	}
}

await File.WriteAllTextAsync(Path.Combine(targetDir, "CTConfig.ts"), ts.ToString());

Console.WriteLine($"Exported CTConfig.json and CTConfig.ts in {start.ElapsedMilliseconds:N0}ms");

return;

static object? ParseValue(ExpressionSyntax? expr, Dictionary<string, object> scope)
	=> expr switch
	{
		LiteralExpressionSyntax lit
			=> lit.Token.Value!,

		BinaryExpressionSyntax bin
			=> EvalBinary(bin, scope),

		ImplicitArrayCreationExpressionSyntax arr
			=> arr.Initializer.Expressions.Select(e => ParseValue(e, scope)).ToArray(),

		CollectionExpressionSyntax col
			=> col.Elements.SelectMany(e => e switch
			{
				ExpressionElementSyntax exprEl
					=> [ParseValue(exprEl.Expression, scope)],
				SpreadElementSyntax spread
					=> ResolveSpread(spread, scope),
				_
					=> Array.Empty<object?>(),
			}).ToArray(),

		not null => expr.ToString(),
		_ => null,
	};

static object?[] ResolveSpread(SpreadElementSyntax spread, Dictionary<string, object> scope)
{
	// The spread expression is typically a simple identifier e.g. `..ImageFileTypes`
	var name = spread.Expression.ToString();
	if (scope.TryGetValue(name, out var value) && value is object?[] arr)
	{
		return arr;
	}
	return [];
}

static object EvalBinary(BinaryExpressionSyntax bin, Dictionary<string, object> scope)
{
	var left = ParseValue(bin.Left, scope);
	var right = ParseValue(bin.Right, scope);

	// Normalize both sides to long so mixed int/long pairs (e.g., the result of
	// a nested binary on the left and a fresh literal on the right) are handled.
	var l = ToLong(left);
	var r = ToLong(right);

	if (l is not null && r is not null)
	{
		return bin.OperatorToken.Text switch
		{
			"*" => l.Value * r.Value,
			"+" => l.Value + r.Value,
			"-" => l.Value - r.Value,
			_ => bin.ToString(),
		};
	}

	return bin.ToString();
}

static long? ToLong(object? value)
	=> value switch
	{
		long l => l,
		int i => i,
		_ => null,
	};

static string ToTs(object? value)
	=> value switch
	{
		string s => $"\"{s}\"",
		bool b => b ? "true" : "false",
		object?[] arr => $"[{string.Join(", ", arr.Select(ToTs))}]",
		_ => value?.ToString() ?? "null",
	};