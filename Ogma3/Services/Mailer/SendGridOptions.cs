using JetBrains.Annotations;

namespace Ogma3.Services.Mailer;

public sealed class SendGridOptions
{
	public required string SendGridUser { get; [UsedImplicitly] init; }
	public required string SendGridKey { get; [UsedImplicitly] init; }
}