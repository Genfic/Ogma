using System;

namespace Ogma3.Pages.Shared;

public class CommentsThreadDto
{
	public required long Id { get; init; }
	public required DateTime? LockDate { get; init; }
	public required string Type { get; set; }
}