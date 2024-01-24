using JetBrains.Annotations;

namespace Ogma3.Services.Mailer;

public class MailGunOptions
{
	public required string MailGunKey { get; [UsedImplicitly] init; }
	public required string MailGunDomain { get; [UsedImplicitly] init; }
}