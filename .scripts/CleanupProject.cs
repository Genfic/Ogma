using System.Collections.Immutable;
using System.Text.RegularExpressions;

Console.WriteLine("Running cleanup...");

const string root = "Ogma3";

var proj = await File.ReadAllLinesAsync(Path.Combine(root, "Ogma3.csproj"));

var removals = proj.Where(l => RegexHelper.ContentIncludedRemove.IsMatch(l.Trim())).ToImmutableHashSet();

var useless = removals.Select(line => {
		var match = RegexHelper.ContentIncludedRemove.Match(line.Trim());

		if (!match.Success) return null;

		var toRemove = match.Groups[1].Value;
		var fullPath = Path.Combine(root, toRemove.Replace('\\', Path.DirectorySeparatorChar));

		return !File.Exists(fullPath)
			? line
			: null;
	})
	.OfType<string>()
	.ToImmutableHashSet();

Console.WriteLine($"Found {useless.Count} useless removals. Continue? (y/n)");

if (Console.ReadLine()?.Trim().ToLower() != "y")
{
	Console.WriteLine("Aborting cleanup.");
	return;
}

var cleaned = proj.Where(l => !useless.Contains(l)).ToImmutableArray();

await File.WriteAllLinesAsync(Path.Combine(root, "Ogma3.csproj"), cleaned);

Console.WriteLine("Cleanup complete.");

return;

internal static partial class RegexHelper
{
	[GeneratedRegex("""<_ContentIncludedByDefault Remove="(.*?)" />""", RegexOptions.Compiled | RegexOptions.Singleline)]
	public static partial Regex ContentIncludedRemove { get; }
}