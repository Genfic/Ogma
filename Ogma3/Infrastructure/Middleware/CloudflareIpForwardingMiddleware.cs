using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using MemoryPack;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Infrastructure.Middleware;

public sealed partial class CloudflareIpForwardingMiddleware
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

		var ranges = await GetCloudflareIpRangesAsync();

		if (!IsCloudflareIp(connectingIp, ranges))
		{
			LogRequestNotFromCloudflare(connectingIp);
			await next(context);
			return;
		}

		var cfConnectingIp = context.Request.Headers["Cf-Connecting-Ip"].FirstOrDefault();

		if (string.IsNullOrEmpty(cfConnectingIp) || !IPAddress.TryParse(cfConnectingIp, out var realIp))
		{
			LogMissingOrInvalidCfHeader(connectingIp);
			await next(context);
			return;
		}

		context.Connection.RemoteIpAddress = realIp;

		await next(context);
	}


	private async Task<CloudflareIpRanges> GetCloudflareIpRangesAsync()
	{
		return await cache.GetOrSetAsync(CacheKey, async ct =>
		{
			var client = clientFactory.CreateClient();

			var ipv4Text = await client.GetStringAsync("https://www.cloudflare.com/ips-v4", ct);
			var ipv6Text = await client.GetStringAsync("https://www.cloudflare.com/ips-v6", ct);

			var ipv4 = ParseRanges(ipv4Text);
			var ipv6 = ParseRanges(ipv6Text);

			LogFetchedCloudflareRanges(ipv4.Length, ipv6.Length);

			return new CloudflareIpRanges
			{
				IPv4 = ipv4,
				IPv6 = ipv6,
			};
		},
		options => options.Duration = TimeSpan.FromDays(1));
	}

	private static IpRange[] ParseRanges(string text)
	{
		var list = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
			.Select(ParseCidr)
			.OfType<IPNetwork>()
			.Select(ConvertCidr)
			.OrderBy(r => r.StartHigh)
			.ThenBy(r => r.StartLow)
			.ToArray();

		return list;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsCloudflareIp(IPAddress ip, CloudflareIpRanges ranges)
	{
		ConvertIp(ip, out var high, out var low);

		var arr = ip.AddressFamily == AddressFamily.InterNetworkV6
			? ranges.IPv6
			: ranges.IPv4;

		var lo = 0;
		var hi = arr.Length - 1;

		while (lo <= hi)
		{
			var mid = lo + ((hi - lo) >> 1);
			ref readonly var r = ref arr[mid];

			if (high < r.StartHigh || (high == r.StartHigh && low < r.StartLow))
			{
				hi = mid - 1;
			}
			else if (high > r.EndHigh || (high == r.EndHigh && low > r.EndLow))
			{
				lo = mid + 1;
			}
			else
			{
				return true;
			}
		}

		return false;
	}

	private static IPNetwork? ParseCidr(string cidr)
	{
		var span = cidr.AsSpan().Trim();
		var slash = span.IndexOf('/');

		if (slash <= 0 || slash == span.Length - 1)
		{
			return null;
		}

		if (!IPAddress.TryParse(span[..slash], out var ip))
		{
			return null;
		}

		if (!int.TryParse(span[(slash + 1)..], out var prefix))
		{
			return null;
		}

		return new IPNetwork(ip, prefix);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ConvertIp(IPAddress ip, out ulong high, out ulong low)
	{
		Span<byte> bytes = stackalloc byte[16];
		ip.TryWriteBytes(bytes, out _);

		high = BinaryPrimitives.ReadUInt64BigEndian(bytes[..8]);
		low  = BinaryPrimitives.ReadUInt64BigEndian(bytes[8..]);
	}

	private static IpRange ConvertCidr(IPNetwork net)
	{
		ConvertIp(net.BaseAddress, out var high, out var low);

		var bits = net.BaseAddress.AddressFamily == AddressFamily.InterNetwork ? 32 : 128;
		var hostBits = bits - net.PrefixLength;

		switch (hostBits)
		{
			case <= 0:
				return new(high, low, high, low);
			case >= 64:
			{
				var shift = hostBits - 64;
				var mask = ulong.MaxValue >> shift;
				return new(high & ~mask, 0, high | mask, ulong.MaxValue);
			}
			default:
			{
				var lowMask = ulong.MaxValue >> hostBits;
				return new(high, low & ~lowMask, high, low | lowMask);
			}
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Request not from Cloudflare IP: {RemoteIp}")]
	private partial void LogRequestNotFromCloudflare(IPAddress? remoteIp);

	[LoggerMessage(Level = LogLevel.Warning, Message = "Request from Cloudflare IP {CloudflareIp} but CF-Connecting-IP header is missing or invalid")]
	private partial void LogMissingOrInvalidCfHeader(IPAddress? cloudflareIp);

	[LoggerMessage(Level = LogLevel.Information, Message = "Fetched {Ipv4Count} IPv4 and {Ipv6Count} IPv6 Cloudflare ranges")]
	private partial void LogFetchedCloudflareRanges(int ipv4Count, int ipv6Count);
}

[MemoryPackable]
public sealed partial class CloudflareIpRanges
{
	public IpRange[] IPv4 { get; init; } = [];
	public IpRange[] IPv6 { get; init; } = [];
}

[MemoryPackable]
public readonly partial struct IpRange(ulong startHigh, ulong startLow, ulong endHigh, ulong endLow)
{
	public ulong StartHigh { get; } = startHigh;
	public ulong StartLow { get; } = startLow;
	public ulong EndHigh { get; } = endHigh;
	public ulong EndLow { get; } = endLow;
}
