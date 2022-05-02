using System;

namespace Ogma3.Pages.Shared.Cards;

public class ThreadCard
{
	public long Id { get; init; }
	public string Title { get; init; }
	public DateTime CreationDate { get; init; }
	public string AuthorName { get; init; }
	public string AuthorAvatar { get; init; }
	public int CommentsCount { get; init; }
	public long ClubId { get; init; }
	public bool IsPinned { get; init; }
}