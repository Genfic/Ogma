using LibGit2Sharp;

namespace Ogma3.AppHost.Helpers;

public static class GitHelpers
{
	public static State GetState()
	{
		using var repo = new Repository(Repository.Discover(AppContext.BaseDirectory));

		var dirty = repo.RetrieveStatus().IsDirty;
		var hash = repo.Head.Tip.Sha;
		var branch = repo.Head.FriendlyName;

		return new State(hash, dirty, branch);
	}

	public sealed record State(string Hash, bool IsDirty, string Branch);
}