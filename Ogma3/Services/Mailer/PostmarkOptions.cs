using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Ogma3.Services.Mailer;

public sealed class PostmarkOptions
{
	public const string Section = "Postmark";

	[Required]
	public required string Key { get; [UsedImplicitly] init; }
	[Required]
	public required string Domain { get; [UsedImplicitly] init; }
}