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
using Ogma3.Services.TagCache;
using Utils.Extensions;

namespace Ogma3.Api.V1.Tags;

using ReturnType = Results<BadRequest<string>, Ok<string>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("tags/bulk")]
[UsedImplicitly]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class BulkCreateTag(AppDbContext context, TagCache cache, ILogger<BulkCreateTag.Handler> logger)
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

		var tags = data
			.Select(kvp => kvp.Value
				.Select(v => new
				{
					Name = v.Transform(To.TitleCase),
					Slug = v.Normalize().Friendlify().ToUpperInvariant(),
					Namespace = kvp.Key.Friendlify().ToLower(),
				}))
			.SelectMany(x => x)
			.DistinctBy(t => t.Slug)
			.DistinctBy(t => t.Name)
			.DistinctBy(t => (t.Name, t.Namespace))
			.ToList();

		var names = new List<string>();
		var slugs = new List<string>();
		var namespaces = new List<string?>();
		foreach (var tag in tags)
		{
			names.Add(tag.Name);
			slugs.Add(tag.Slug);
			namespaces.Add(tag.Namespace);
		}

		var inserted = await context.Database.SqlQuery<TagEntry>(// lang=sql
			$"""
			INSERT INTO "Tags" ("Name", "Slug", "NamespaceId")
			SELECT u.name, u.slug, n."Id"
			FROM UNNEST({names}, {slugs}, {namespaces}) AS u(name, slug, ns_slug)
			JOIN "TagNamespace" n ON n."Slug" = u.ns_slug
			ON CONFLICT DO NOTHING
			RETURNING "Id", "Slug", n."Name";
			""")
			.ToListAsync(cancellationToken);

		await cache.AddManyAsync(inserted);

		logger.LogInformation("Bulk inserted {Inserted} tags", inserted);

		return TypedResults.Ok($"Created {inserted} of {data.Sum(p => p.Value.Count)} tags");
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