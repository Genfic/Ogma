using JetBrains.Annotations;

namespace Ogma3.Services.Mailer;

public sealed class PostmarkOptions
{
	public required string Key { get; [UsedImplicitly] init; }
	public required string Domain { get; [UsedImplicitly] init; }
}