using System.Linq.Expressions;
using Ogma3.Data.Tags;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Data.Stories;

public static class StoryMapper
{
	public static readonly Expression<Func<Story, StoryCard>> MapToCard = s => new StoryCard
	{
		Id = s.Id,
		AuthorUserName = s.Author.UserName,
		Title = s.Title,
		Slug = s.Slug,
		Hook = s.Hook,
		Cover = s.Cover.Url,
		PublicationDate = s.PublicationDate,
		Tags = s.Tags.OrderBy(t => t.Namespace).ThenBy(t => t.Name).Select(t => t.ToDto()),
		Rating = s.Rating,
		Status = s.Status,
		WordCount = s.WordCount,
		ChapterCount = s.ChapterCount,
	};

	public static readonly Expression<Func<Story, StoryDetails>> MapToDetails = s => new StoryDetails
	{
		Id = s.Id,
		AuthorId = s.AuthorId,
		Title = s.Title,
		Slug = s.Slug,
		Description = s.Description,
		Hook = s.Hook,
		Cover = s.Cover.Url,
		ReleaseDate = s.PublicationDate ?? s.CreationDate,
		IsPublished = s.PublicationDate != null,
		Tags = s.Tags.OrderBy(t => t.Namespace).ThenBy(t => t.Name).Select(t => t.ToDto()),
		Rating = s.Rating,
		Status = s.Status,
		IsLocked = s.IsLocked,
		WordCount = s.WordCount,
		ChaptersCount = s.Chapters.Count,
		CommentsCount = s.Chapters.Sum(c => c.CommentThread.CommentsCount),
		VotesCount = s.Votes.Count,
		ContentBlock = s.ContentBlock != null ? new Pages.Shared.ContentBlockCard(
			s.ContentBlock.Reason ?? "",
			s.ContentBlock.DateTime,
			s.ContentBlock.Issuer != null ? s.ContentBlock.Issuer.UserName : ""
		) : null,
		Credits = s.Credits.Select(c => new CreditDto(c.Role, c.Name, c.Link)).ToList(),
	};
}