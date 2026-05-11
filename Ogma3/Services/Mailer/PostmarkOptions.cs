using ConfigBoundNET;

namespace Ogma3.Services.Mailer;

[ConfigSection("Postmark")]
public sealed partial class PostmarkOptions
{
	public required string Key { get; init; }
	public required string Domain { get; init; }
}