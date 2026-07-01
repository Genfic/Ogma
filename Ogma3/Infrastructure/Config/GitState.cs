using ConfigBinder.Attributes;
using Immediate.Validations.Shared;

namespace Ogma3.Infrastructure.Config;

[Validate]
[ConfigSection("Git")]
public sealed partial class GitState : IValidationTarget<GitState>
{
	public required string CommitHash { get; init; }
	public required string Branch { get; init; }
	public required bool Dirty { get; init; }
}