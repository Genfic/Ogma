using System;

namespace Ogma3.Pages.Shared.Cards;

public class ThreadCard
{
	public required long Id { get; init; }
	public required string Title { get; init; }
	public required DateTime CreationDate { get; init; }
	public required string AuthorName { get; init; }
	public required string AuthorAvatar { get; init; }
	public required int CommentsCount { get; init; }
	public required long ClubId { get; init; }
	public required bool IsPinned { get; init; }
}