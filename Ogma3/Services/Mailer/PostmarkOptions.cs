using ConfigBinder.Attributes;
using Immediate.Validations.Shared;

namespace Ogma3.Services.Mailer;

[Validate]
[ConfigSection("Postmark")]
public sealed partial class PostmarkOptions : IValidationTarget<PostmarkOptions>
{
	public required string Key { get; init; }
	public required string Domain { get; init; }
}