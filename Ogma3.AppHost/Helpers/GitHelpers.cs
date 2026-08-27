using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;

namespace Ogma3.AppHost.Helpers;

public static class GitHelpers
{
	public static GitState RegisterAndGetGitState(this IDistributedApplicationBuilder builder)
	{
		using var repo = new Repository(Repository.Discover(AppContext.BaseDirectory));

		var dirty = repo.RetrieveStatus().IsDirty;
		var hash = repo.Head.Tip.Sha;
		var branch = repo.Head.FriendlyName;

		var state = new GitState(hash, dirty, branch);

		builder.Services.AddSingleton(state);

		return state;
	}

	public static GitState GetGitState(this IServiceProvider services) => services.GetRequiredService<GitState>();

	public sealed record GitState(string Hash, bool IsDirty, string Branch);
}