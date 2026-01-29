using System.Collections.Frozen;
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

using ReturnType = Results<Ok, NotFound, BadRequest, InternalServerError>;

[Handler]
[MapPost("hooks/patreon")]
[AllowAnonymous]
public static partial class HandlePatreonWebhook
{
	private static readonly FrozenSet<string> AllowedEvents =
	[
		"members:create",
		"members:update",
		"members:delete",
		"members:pledge:create",
		"members:pledge:update",
		"members:pledge:delete",
	];

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
		if (!AllowedEvents.Contains(request.Event))
		{
			return TypedResults.Ok();
		}

		if (config["Webhooks:Patreon:Secret"] is not {} secret)
		{
			logger.LogCritical("Patreon webhook secret is not set.");
			return TypedResults.InternalServerError();
		}

		if (httpContextAccessor.HttpContext is not {} httpContext)
		{
			return TypedResults.InternalServerError();
		}

		logger.LogInformation("Patreon webhook for {Event} received.", request.Event);

		using var ms = new MemoryStream(4096); // 4096 should be enough for the webhook payload
		await httpContext.Request.Body.CopyToAsync(ms, cancellationToken);
		var body = ms.GetBuffer().AsSpan(0, (int)ms.Length);

		if (!ValidateSignature(request.Signature, secret, body))
		{
			logger.LogWarning("Invalid Patreon webhook signature: {Signature}", request.Signature);
			return TypedResults.BadRequest();
		}

		PatreonWebhook? payload;
		try
		{
			payload = JsonSerializer.Deserialize(body, PatreonWebhookContext.Default.PatreonWebhook);
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to deserialize Patreon webhook payload");
			return TypedResults.BadRequest();
		}

		if (payload is null)
		{
			logger.LogWarning("Invalid Patreon webhook payload");
			return TypedResults.BadRequest();
		}

		var patreonUserId = payload.Data.Relationships.User.Data.Id;
		var entitledCents = payload.Data.Attributes.CurrentlyEntitledAmountCents;
		var tierIds = payload.Data.Relationships.CurrentlyEntitledTiers.Data.Select(x => x.Id).ToList();
		var status = payload.Data.Attributes.PatronStatus;

		var user = await userManager.FindByLoginAsync("Patreon", patreonUserId);
		if (user is null)
		{
			logger.LogWarning("Patreon webhook payload contained nonexistent user id: {UserId}", patreonUserId);

			// just to be sure, let's delete any subscriptions that might be tied to this ID if the event is deletion
			if (request.Event is "members:pledge:delete" or "members:delete" or "members:pledge:update" or "members:update")
			{
				var rows = await context.Subscriptions
					.Where(s => s.PatreonUserId == patreonUserId)
					.ExecuteDeleteAsync(cancellationToken);

				logger.LogInformation("Deleted {Rows} subscriptions tied to deleted Patreon user {UserId}.", rows, patreonUserId);
			}

			return TypedResults.NotFound();
		}

		if (entitledCents <= 0 || status is "former_patron")
		{
			var rows = await context.Subscriptions
				.Where(s => s.PatreonUserId == patreonUserId)
				.ExecuteDeleteAsync(cancellationToken);
			logger.LogInformation("Deleted {Rows} subscriptions tied to deleted user {UserId} (Patreon: {PatreonId}).", rows, user.Id, patreonUserId);
		}

		var subscriptionExists = await context.Subscriptions
			.Where(s => s.UserId == user.Id)
			.AnyAsync(cancellationToken);

		var tierId = await context.SubscriptionTiers
			.OrderByDescending(t => t.AmountCents)
			.Where(t => t.AmountCents <= entitledCents)
			.Select(t => (long?)t.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (tierId is null)
		{
			logger.LogWarning("No tier matched {Entitlement} cents of entitlement", entitledCents);
			return TypedResults.Ok();
		}

		if (subscriptionExists)
		{
			await context.Subscriptions
				.Where(s => s.UserId == user.Id)
				.ExecuteUpdateAsync(set => set
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
				PatreonUserId = patreonUserId,
				UserId = user.Id,
				TierId = tierId,
			};
			context.Subscriptions.Add(sub);
			await context.SaveChangesAsync(cancellationToken);
		}

		return TypedResults.Ok();
	}

	private static bool ValidateSignature(string signature, string key, ReadOnlySpan<byte> body)
	{
		// MD5 produces 16 bytes, so 32 hex chars
		if (signature.Length != 32)
		{
			return false;
		}

		Span<byte> signatureBytes = stackalloc byte[16];

		try
		{
			Convert.FromHexString(signature, signatureBytes, out _, out _);
		}
		catch
		{
			return false;
		}

		var keyBytes = Encoding.UTF8.GetBytes(key);
		using var hmac = new HMACMD5(keyBytes);

		Span<byte> computedHash = stackalloc byte[16];

		return hmac.TryComputeHash(body, computedHash, out _) && CryptographicOperations.FixedTimeEquals(signatureBytes, computedHash);
	}
}