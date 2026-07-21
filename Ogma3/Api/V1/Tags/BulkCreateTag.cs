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
public sealed partial class BulkCreateTag(ApplicationDbContext context, TagCache cache, ILogger<BulkCreateTag.Handler> logger)
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
				.Select(v => new Tag
				{
					Name = v.Transform(To.TitleCase),
					Slug = v.Normalize().Friendlify().ToUpperInvariant(),
					Namespace = string.IsNullOrWhiteSpace(kvp.Key) ? null : ETagNamespaceExtensions.Parse(kvp.Key, true),
				}))
			.SelectMany(x => x)
			.DistinctBy(t => t.Slug)
			.DistinctBy(t => t.Name)
			.DistinctBy(t => (t.Name, t.Namespace))
			.ToList();

		var names = new List<string>();
		var slugs = new List<string>();
		var namespaces = new List<ETagNamespace?>();
		foreach (var tag in tags)
		{
			names.Add(tag.Name);
			slugs.Add(tag.Slug);
			namespaces.Add(tag.Namespace);
		}

		var inserted = await context.Database.ExecuteSqlAsync(// lang=sql
			$"""
			INSERT INTO "Tags" ("Name", "Slug", "Namespace")
			SELECT * FROM UNNEST({names}, {slugs}, {namespaces}::"e_tag_namespace"[])
			ON CONFLICT DO NOTHING;
			""",
			cancellationToken);

		await cache.AddManyAsync(tags.Select(t => new TagEntry(t.Id, t.Name, t.Namespace)));

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