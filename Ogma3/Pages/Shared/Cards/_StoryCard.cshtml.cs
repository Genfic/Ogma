#nullable enable


using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;

namespace Ogma3.Pages.Shared.Cards;

public class StoryCard
{
	public long Id { get; set; }
	public string AuthorUserName { get; set; } = null!;
	public string Title { get; set; } = null!;
	public string Slug { get; set; } = null!;
	public string Hook { get; set; } = null!;
	public string? Cover { get; set; }

	public string? CoverId { get; set; }
	public DateTime? PublicationDate { get; set; }
	public IList<TagDto> Tags { get; set; } = null!;
	public Rating Rating { get; set; } = null!;
	public EStoryStatus Status { get; set; }
	public int WordCount { get; set; }
	public int ChapterCount { get; set; }

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Story, StoryCard>()
			.ForMember(sc => sc.Tags, opts
				=> opts.MapFrom(s => s.Tags.OrderByDescending(t => t.Namespace.HasValue).ThenByDescending(t => t.Namespace))
			);
	}
}