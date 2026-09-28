#! usr/bin/env dotnet
#:package LibGit2Sharp@0.32.0

using System.Diagnostics;
using LibGit2Sharp;

using Semver = System.Version;

if (args is not ["major" or "minor" or "patch"])
{
	Console.Error.WriteLine("Usage: Bump.cs [major|minor|patch]");
	return 1;
}

var repoPath = Repository.Discover(Environment.CurrentDirectory);
if (repoPath is null)
{
	Console.Error.WriteLine("Could not find git repository");
	return 1;
}

using var repo = new Repository(repoPath);

var tags = repo.Tags
	.Where(t => t.FriendlyName.StartsWith('v'))
	.Select(t => Semver.TryParse(t.FriendlyName[1..], out var v) ? v : null)
	.OfType<Semver>()
	.OrderByDescending(v => v)
	.ToList();

var current = tags.Count > 0 
	? tags[0] 
	: new Semver(0, 0, 0);

var next = args[0] switch {
	"major" => new Semver(current.Major + 1, 0, 0),
	"minor" => new Semver(current.Major, current.Minor + 1, 0),
	"patch" => new Semver(current.Major, current.Minor, current.Build + 1),
	_ => throw new UnreachableException(),
};

Console.WriteLine($"Bumping from v{current} to v{next}");
Console.WriteLine("Press [enter] to accept, press [ctrl+c] to cancel");
Console.ReadLine();

repo.ApplyTag($"v{next}");

return 0;
