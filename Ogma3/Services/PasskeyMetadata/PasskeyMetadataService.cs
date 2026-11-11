using System.Buffers.Text;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Text.Json;
using Immediate.Injections.Shared;
using Microsoft.AspNetCore.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Services.PasskeyMetadata;

[RegisterSingleton]
public sealed class PasskeyMetadataService(IFusionCache cache, IHttpClientFactory clientFactory, ILogger<PasskeyMetadataService> logger)
{
	private const string CmdsUrl = "https://c-mds.fidoalliance.org/";
	private const string CacheKeyNames = "passkey:c-mds-names";
	private const string CacheKeySerial = "passkey:c-mds-serial";
	private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(30);

	// Final snapshot of the (deprecated) community GitHub list, bundled as the fallback layer
	// per the RP-fallback migration instructions: https://github.com/passkeydeveloper/passkey-authenticator-aaguids/wiki/RP-Fallback
	private static readonly Dictionary<Guid, string> Snapshot = LoadSnapshot();

	/// <summary>AAGUID → authenticator name. C-MDS entries take precedence over the bundled snapshot.</summary>
	public FrozenDictionary<Guid, string> Names { get; private set; } = Snapshot.ToFrozenDictionary();

	/// <summary>Serial number of the currently cached C-MDS blob, as reported by its ETag header.</summary>
	public string? Serial { get; private set; }

	private Dictionary<Guid, string> _cmds = [];

	public string GetDisplayName(UserPasskeyInfo passkey)
	{
		if (!string.IsNullOrEmpty(passkey.Name))
		{
			return passkey.Name;
		}

		return TryGetDefaultDisplayName(passkey, out var name) ? name : "Unnamed passkey";
	}

	public bool TryGetDefaultDisplayName(UserPasskeyInfo passkey, [NotNullWhen(true)] out string? name)
	{
		name = null;

		if (passkey.Aaguid is not { Length: 16 } aaguid)
		{
			return false;
		}

		var guid = new Guid(aaguid, bigEndian: true);
		if (guid == Guid.Empty)
		{
			return false; // some authenticators report a zeroed AAGUID
		}

		return Names.TryGetValue(guid, out name);
	}

	/// <summary>
	/// Restores the last cached C-MDS names (if any) and then attempts a refresh.
	/// Non-fatal: on any failure the bundled snapshot stays in effect and the job retries daily.
	/// </summary>
	public async Task WarmUpAsync(CancellationToken ct)
	{
		if (Snapshot.Count == 0)
		{
			logger.LogWarning("The bundled passkey AAGUID snapshot (passkey_aaguids.json) is missing or empty");
		}

		try
		{
			var names = await cache.GetOrDefaultAsync<Dictionary<Guid, string>>(CacheKeyNames, token: ct);
			var serial = await cache.GetOrDefaultAsync<string>(CacheKeySerial, token: ct);
			if (names is { Count: > 0 })
			{
				Apply(serial, names);
			}
		}
		catch (Exception e)
		{
			logger.LogWarning(e, "Could not restore cached C-MDS passkey metadata; using the bundled snapshot");
		}

		// Bound the initial fetch so a slow or unreachable C-MDS can't delay startup for long
		using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
		timeout.CancelAfter(TimeSpan.FromSeconds(15));
		await RefreshAsync(timeout.Token);
	}

	/// <summary>
	/// Conditionally fetches the C-MDS blob. The cached serial is passed as <c>localCopySerial</c>;
	/// the server responds 304 Not Modified when the blob is unchanged.
	/// </summary>
	public async Task RefreshAsync(CancellationToken ct)
	{
		try
		{
			var url = Serial is { } serial ? $"{CmdsUrl}?localCopySerial={Uri.EscapeDataString(serial)}" : CmdsUrl;
			using var request = new HttpRequestMessage(HttpMethod.Get, url);
			using var client = clientFactory.CreateClient();
			using var response = await client.SendAsync(request, ct);

			if (response.StatusCode == HttpStatusCode.NotModified)
			{
				await TouchCacheAsync();
				return;
			}

			response.EnsureSuccessStatusCode();

			var etag = response.Headers.ETag?.Tag.Trim('"');
			var blob = await response.Content.ReadAsStringAsync(ct);
			var json = GetPayloadJson(blob);
			var parsed = JsonSerializer.Deserialize(json, PasskeyMetadataContext.Default.CmdsBlob)
			             ?? throw new FormatException("The C-MDS blob deserialized to null");

			var cmds = ExtractNames(parsed);

			if (cmds.Count == 0)
			{
				logger.LogWarning("The C-MDS blob contained no usable AAGUID names; keeping previously known data");
				return;
			}

			Apply(etag ?? (parsed.No is { } no ? no.ToString() : null), cmds);
			await CacheAsync(cmds);
		}
		catch (Exception e) when (ct.IsCancellationRequested is false)
		{
			logger.LogWarning(e, "Failed to fetch the C-MDS passkey metadata blob; keeping previously known data");
		}
	}

	private void Apply(string? serial, Dictionary<Guid, string> cmds)
	{
		Serial = serial;
		_cmds = cmds;
		var merged = new Dictionary<Guid, string>(Snapshot);

		foreach (var (guid, name) in cmds)
		{
			merged[guid] = name;
		}

		Names = merged.ToFrozenDictionary();

		logger.LogInformation("Passkey AAGUID metadata updated: {Count} entries (C-MDS serial {Serial})", Names.Count, serial ?? "n/a");
	}

	private async Task CacheAsync(Dictionary<Guid, string> cmds)
	{
		await cache.SetAsync(CacheKeyNames, cmds, o => o.Duration = CacheDuration);
		if (Serial is { } serial)
		{
			await cache.SetAsync(CacheKeySerial, serial, o => o.Duration = CacheDuration);
		}
	}

	/// <summary>The cached copy is still current (304); extend its lifetime instead of re-downloading.</summary>
	private async Task TouchCacheAsync()
	{
		if (_cmds.Count == 0 || Serial is not { } serial)
		{
			return;
		}

		await cache.SetAsync(CacheKeyNames, _cmds, o => o.Duration = CacheDuration);
		await cache.SetAsync(CacheKeySerial, serial, o => o.Duration = CacheDuration);
		logger.LogDebug("C-MDS metadata unchanged (serial {Serial}); cached copy refreshed", serial);
	}

	private static string GetPayloadJson(string blob)
	{
		if (blob.TrimStart().StartsWith('{'))
		{
			return blob;
		}

		return blob.Split('.') is [_, var payload, ..]
			? Encoding.UTF8.GetString(Base64Url.DecodeFromChars(payload))
			: throw new FormatException("The C-MDS blob does not have the expected JSON or header.payload form");

	}

	private static Dictionary<Guid, string> ExtractNames(CmdsBlob blob)
	{
		var result = new Dictionary<Guid, string>();
		foreach (var (aaguid, entry) in blob.Entries)
		{
			if (!Guid.TryParse(aaguid, out var guid) || guid == Guid.Empty)
			{
				continue;
			}

			var name = GetFriendlyName(entry.FriendlyNames);
			if (!string.IsNullOrWhiteSpace(name))
			{
				result[guid] = name;
			}
		}
		return result;
	}

	private static string? GetFriendlyName(Dictionary<string, string>? friendlyNames)
	{
		if (friendlyNames is null)
		{
			return null;
		}

		foreach (var (language, value) in friendlyNames)
		{
			if (string.Equals(language, "en-US", StringComparison.OrdinalIgnoreCase))
			{
				return value;
			}
		}

		// The spec requires "en-US" to always be present; tolerate other en-* variants anyway
		foreach (var (language, value) in friendlyNames)
		{
			if (language.StartsWith("en-", StringComparison.OrdinalIgnoreCase))
			{
				return value;
			}
		}

		return null;
	}

	private static Dictionary<Guid, string> LoadSnapshot()
	{
		try
		{
			return JsonSerializer.Deserialize(File.ReadAllText("passkey_aaguids.json"), PasskeyMetadataContext.Default.DictionaryGuidString)
				?? [];
		}
		catch (FileNotFoundException)
		{
			return [];
		}
		catch (DirectoryNotFoundException)
		{
			return [];
		}
	}
}
