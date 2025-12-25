using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Subscriptions;

namespace Ogma3.Api.Hooks.Patreon;

using ReturnType = Results<Ok, UnauthorizedHttpResult, NotFound, BadRequest<string>, InternalServerError>;

[Handler]
[MapPost("hooks/patreon")]
[AllowAnonymous]
public static partial class HandlePatreonWebhook
{
	[UsedImplicitly]
	public sealed record Query
	(
		[property: FromHeader(Name = "X-Patreon-Signature")] string Signature,
		[property: FromHeader(Name = "X-Patreon-Event")] string Event
	);

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		IConfiguration config,
		ApplicationDbContext context,
		IHttpContextAccessor httpContextAccessor,
		OgmaUserManager userManager,
		ILogger<Query> logger,
		CancellationToken cancellationToken
	)
	{
		if (config["Webhooks:Patreon:Secret"] is not {} secret)
		{
			logger.LogCritical("Patreon webhook secret is not set.");
			return TypedResults.InternalServerError();
		}

		if (httpContextAccessor.HttpContext is not {} httpContext)
		{
			return TypedResults.InternalServerError();
		}

		using var reader = new StreamReader(httpContext.Request.Body);
		var body = await reader.ReadToEndAsync(cancellationToken);

		if (!ValidateSignature(request.Signature, secret, body))
		{
			return TypedResults.Unauthorized();
		}

		var payload = JsonSerializer.Deserialize(body, PatreonWebhookContext.Default.PatreonWebhook);
		if (payload is null)
		{
			logger.LogWarning("Patreon webhook payload was invalid. {Payload}", body);
			return TypedResults.BadRequest("Unexpected payload format.");
		}

		var patreonUserId = payload.Data.Data.Attributes.Relationships.User.Data.Id;
		var entitledCents = payload.Data.Data.Attributes.CurrentlyEntitledAmountCents;
		var tierIds = payload.Data.Data.Attributes.Relationships.CurrentlyEntitledTiers.Data.Select(x => x.Id).ToList();
		var status = payload.Data.Data.Attributes.PatronStatus;

		var user = await userManager.FindByLoginAsync("Patreon", patreonUserId);
		if (user is null)
		{
			logger.LogWarning("Patreon webhook payload contained nonexistent user id: {UserId}", patreonUserId);
			return TypedResults.NotFound();
		}

		var subscriptionExists = await context.Subscriptions
			.Where(s => s.UserId == user.Id)
			.AnyAsync(cancellationToken);

		var tierId = await context.SubscriptionTiers
			.OrderByDescending(t => t.AmountCents)
			.Where(t => t.AmountCents <= entitledCents)
			.Select(t => (long?)t.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (subscriptionExists)
		{
			await context.Subscriptions.ExecuteUpdateAsync(set => set
					.SetProperty(s => s.PatreonStatus, status)
					.SetProperty(s => s.PatreonTierIds, tierIds)
					.SetProperty(s => s.TierId, tierId)
					.SetProperty(s => s.LastChange, DateTimeOffset.UtcNow),
				cancellationToken);
		}
		else
		{
			var sub = new Subscription
			{
				PatreonStatus = status,
				PatreonTierIds = tierIds,
				UserId = user.Id,
				TierId = tierId,
			};
			context.Subscriptions.Add(sub);
		}
		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok();
	}

	private static bool ValidateSignature(string signature, string key, string body)
	{
		var keyBytes = Encoding.UTF8.GetBytes(key);
		var bodyBytes = Encoding.UTF8.GetBytes(body);

		using var hmac = new HMACMD5(keyBytes);
		var hashBytes = hmac.ComputeHash(bodyBytes);

		var calculatedSignature = Convert.ToHexString(hashBytes);

		return string.Equals(signature, calculatedSignature, StringComparison.OrdinalIgnoreCase);
	}
}