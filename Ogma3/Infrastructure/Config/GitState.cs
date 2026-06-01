using ConfigBoundNET;

namespace Ogma3.Infrastructure.Config;

[ConfigSection("Git")]
public sealed partial class GitState
{
	public required string CommitHash { get; init; }
	public required string Branch { get; init; }
	public required bool Dirty { get; init; }
}