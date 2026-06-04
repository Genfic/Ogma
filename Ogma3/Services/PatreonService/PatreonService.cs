using System.Net.Http.Headers;
using System.Text.Json;
using Immediate.Injections.Shared;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Config;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Services.PatreonService;

[RegisterSingleton]
public sealed class PatreonService
(
	IHttpClientFactory clientFactory,
	IOptions<Patreon> options,
	ILogger<PatreonService> logger,
	IFusionCache cache
)
{
	private readonly HttpClient _client = clientFactory.CreateClient();

	public async Task<IReadOnlyList<PatreonTier>> GetCampaignTiers(bool bypassCache = false)
	{
		if (!bypassCache)
		{
			return await cache.GetOrSetAsync(
				"patreon:tiers",
				await GetCampaignTiersPrivate(),
				opts => {
					opts.Duration = TimeSpan.FromDays(7);
				}
			);
		}

		var tiers = await GetCampaignTiersPrivate();
		await cache.SetAsync("patreon:tiers", tiers);
		return tiers;
	}

	private async Task<IReadOnlyList<PatreonTier>> GetCampaignTiersPrivate()
	{
		var url =
			$"https://www.patreon.com/api/oauth2/v2/campaigns/{options.Value.CampaignId}?include=tiers.benefits&fields[tier]=title,amount_cents,url&fields[benefit]=title";
		var req = new HttpRequestMessage(HttpMethod.Get, url);
		req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.AccessToken);

		var res = await _client.SendAsync(req);

		res.EnsureSuccessStatusCode();

		var str = await res.Content.ReadAsStringAsync();

		logger.LogInformation("Patreon campaign data: {Data}", str);

		var data = JsonSerializer.Deserialize(str, PatreonDataContext.Default.CampaignData);

		if (data is null)
		{
			logger.LogError("Patreon tier data was null");
			return [];
		}

		var benefits = data.Included
			.OfType<BenefitInclusion>()
			.ToDictionary(i => i.Id, i => new PatreonBenefit
			{
				Name = i.Attributes.Title,
				Description = i.Attributes.Description,
			});

		var tiers = data.Included
			.OfType<TierInclusion>()
			.Select(i => {
				var bens = (i.Relationships?.Benefits.Data ?? [])
					.Select(datum => benefits[datum.Id])
					.ToList();

				return new PatreonTier
				{
					Name = i.Attributes.Title,
					Description = i.Attributes.Description,
					AmountCents = i.Attributes.AmountCents,
					Url = i.Attributes.Url,
					Benefits = bens,
				};
			})
			.Where(t => t.AmountCents > 0)
			.ToList();

		return tiers.AsReadOnly();
	}
}