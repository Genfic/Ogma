using System.Net;
using System.Net.Sockets;
using MemoryPack;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Infrastructure.Middleware;

public sealed class CloudflareIpForwardingMiddleware
(
	IFusionCache cache,
	ILogger<CloudflareIpForwardingMiddleware> logger,
	IHttpClientFactory clientFactory
) : IMiddleware
{
	private const string CacheKey = "cloudflare-ip-ranges";

	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var connectingIp = context.Connection.RemoteIpAddress;

		if (connectingIp is null)
		{
			await next(context);
			return;
		}

		var cloudflareRanges = await GetCloudflareIpRangesAsync();

		if (!IsCloudflareIp(connectingIp, cloudflareRanges))
		{
			logger.LogDebug("Request not from Cloudflare IP: {RemoteIp}", connectingIp);
			await next(context);
			return;
		}

		var cfConnectingIp = context.Request.Headers["Cf-Connecting-Ip"].FirstOrDefault();

		if (string.IsNullOrEmpty(cfConnectingIp) || !IPAddress.TryParse(cfConnectingIp, out var realIp))
		{
			logger.LogWarning("Request from Cloudflare IP {CloudflareIp} but CF-Connecting-IP header is missing or invalid",
				connectingIp);
			await next(context);
			return;
		}

		logger.LogDebug("Setting RemoteIpAddress from CF-Connecting-IP: {RealIp} (was {CloudflareIp})", realIp, connectingIp);

		context.Connection.RemoteIpAddress = realIp;

		await next(context);
	}

	private async Task<CloudflareIpRanges> GetCloudflareIpRangesAsync()
	{
		return await cache.GetOrSetAsync(CacheKey, async ct => {
				var client = clientFactory.CreateClient();

				// Fetch IPv4 ranges
				var ipv4Response = await client.GetStringAsync("https://www.cloudflare.com/ips-v4", ct);
				var ipv4Ranges = ipv4Response.Split('\n', StringSplitOptions.RemoveEmptyEntries)
					.Select(ParseCidr)
					.OfType<IPNetwork>()
					.Select(SerializableIpNetwork.FromIpNetwork)
					.ToList();

				// Fetch IPv6 ranges
				var ipv6Response = await client.GetStringAsync("https://www.cloudflare.com/ips-v6", ct);
				var ipv6Ranges = ipv6Response.Split('\n', StringSplitOptions.RemoveEmptyEntries)
					.Select(ParseCidr)
					.OfType<IPNetwork>()
					.Select(SerializableIpNetwork.FromIpNetwork)
					.ToList();

				logger.LogInformation("Successfully fetched {Ipv4Count} IPv4 and {Ipv6Count} IPv6 Cloudflare ranges", ipv4Ranges.Count,
					ipv6Ranges.Count);

				return new CloudflareIpRanges
				{
					IPv4Ranges = ipv4Ranges,
					IPv6Ranges = ipv6Ranges,
				};
			},
			options => options.Duration = TimeSpan.FromDays(1));
	}

	private IPNetwork? ParseCidr(string cidr)
	{
		try
		{
			var parts = cidr.Trim().Split('/');
			if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var address) && int.TryParse(parts[1], out var prefixLength))
			{
				return new IPNetwork(address, prefixLength);
			}
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to parse CIDR: {Cidr}", cidr);
		}
		return null;
	}

	private static bool IsCloudflareIp(IPAddress ipAddress, CloudflareIpRanges ranges)
	{
		var rangeList = ipAddress.AddressFamily == AddressFamily.InterNetworkV6
			? ranges.IPv6Ranges
			: ranges.IPv4Ranges;

		return rangeList.Any(range => range.ToIpNetwork().Contains(ipAddress));
	}
}

[MemoryPackable]
public sealed partial class CloudflareIpRanges
{
	public List<SerializableIpNetwork> IPv4Ranges { get; init; } = [];
	public List<SerializableIpNetwork> IPv6Ranges { get; init; } = [];
}

[MemoryPackable]
public sealed partial record SerializableIpNetwork(string BaseAddress, int PrefixLength)
{

	public IPNetwork ToIpNetwork() => new(IPAddress.Parse(BaseAddress), PrefixLength);

	public static SerializableIpNetwork FromIpNetwork(IPNetwork network)
		=> new(
			network.BaseAddress.ToString(),
			network.PrefixLength
		);
}