using System;

namespace Ogma3.Pages.Shared;

public class ContentBlockCard
{
	public required string Reason { get; init; }
	public required string IssuerUserName { get; init; }
	public required DateTime DateTime { get; init; }
}