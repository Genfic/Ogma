using System.Text.Json;
using System.Text.Json.Serialization;
using Humanizer;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.ServiceRegistrations;
using Utils.Extensions;

namespace Ogma3.Api.V1.Tags;

using ReturnType = Results<BadRequest<string>, Ok<string>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("tags/bulk")]
[UsedImplicitly]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class BulkCreateTag(ApplicationDbContext context, ILogger<BulkCreateTag.Handler> logger)
{
	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var data = JsonSerializer.Deserialize(request.Json, JsonContext.Default.DictionaryStringListString);

		if (data is null)
		{
			return TypedResults.BadRequest("Incorrect file format.");
		}

		var existing = await context.Tags
			.Select(t => t.Slug)
			.ToListAsync(cancellationToken);

		var tags = data
			.Select(kvp => kvp.Value
				.Select(v => new Tag
				{
					Name = v.Transform(To.TitleCase),
					Slug = v.Normalize().Friendlify().ToUpperInvariant(),
					Namespace = string.IsNullOrWhiteSpace(kvp.Key) ? null : ETagNamespaceExtensions.Parse(kvp.Key, true),
				}))
			.SelectMany(x => x)
			.DistinctBy(t => t.Slug)
			.ExceptBy(
				existing,
				t => t.Slug
			)
			.ToList();

		logger.LogInformation("Bulk-inserted {Tags} new tags.", tags.Count);

		context.Tags.AddRange(tags);
		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok($"Created {tags.Count} of {data.Sum(p => p.Value.Count)} tags");
	}

	[UsedImplicitly]
	public sealed record Command(string Json);

	[JsonSerializable(typeof(Dictionary<string, List<string>>))]
	[JsonSourceGenerationOptions(
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling = JsonCommentHandling.Skip,
		AllowTrailingCommas = true
	)]
	private sealed partial class JsonContext : JsonSerializerContext;
}