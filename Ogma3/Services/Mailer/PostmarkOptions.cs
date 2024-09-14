using JetBrains.Annotations;

namespace Ogma3.Services.Mailer;

public sealed class PostmarkOptions
{
	public required string PostmarkKey { get; [UsedImplicitly] init; }
	public required string PostmarkDomain { get; [UsedImplicitly] init; }
}