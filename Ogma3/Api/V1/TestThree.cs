using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Subscriptions;
using Ogma3.Infrastructure.OgmaConfig;

namespace Ogma3.Api.V1;

using ReturnType = Results<Ok<List<string>>, NotFound>;

[Handler]
[MapGet("api/test-three")]
public sealed partial class TestThree(OgmaConfig config, ApplicationDbContext ctx)
{
	private async ValueTask<ReturnType> Handle(Query q, CancellationToken ct)
	{
		var safeUsers = ctx.Subscriptions
			.Where(s => s.Tier != null && (s.Tier.Entitlements & Entitlement.DraftsLastForever) != 0)
			.Select(s => s.UserId);

		var longerUsers = ctx.Subscriptions
			.Where(s => s.Tier != null && (s.Tier.Entitlements & Entitlement.DraftsLastLonger) != 0)
			.Select(s => s.UserId);

		var now = DateTimeOffset.UtcNow;
		var chapterCount = ctx.Chapters
			.Where(c => c.PublicationDate == null)
			.Where(c => !safeUsers.Contains(c.Story.AuthorId))
			.Where(c => c.CreationDate < (longerUsers.Contains(c.Story.AuthorId)
				? now.AddDays(-config.PremiumDraftRetentionDays)
				: now.AddDays(-config.DraftRetentionDays)))
			.ToQueryString();

		var blogpostCount = ctx.Blogposts
			.Where(b => b.PublicationDate == null)
			.Where(b => !safeUsers.Contains(b.AuthorId))
			.Where(b => b.CreationDate < (longerUsers.Contains(b.AuthorId)
				? now.AddDays(-config.PremiumDraftRetentionDays)
				: now.AddDays(-config.DraftRetentionDays)))
			.ToQueryString();

		return TypedResults.Ok(new List<string> { chapterCount, blogpostCount });
	}

	public sealed record Query;
}