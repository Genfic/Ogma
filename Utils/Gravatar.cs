using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using NetEscapades.EnumGenerators;

namespace Utils;

public static class Gravatar
{
	public static string Generate(string email, Options? options = null)
	{
		var normalized = email.Trim().ToLower();

		Span<byte> hash = stackalloc byte[MD5.HashSizeInBytes];
		MD5.TryHashData(Encoding.UTF8.GetBytes(normalized), hash, out _);

		var baseUrl = $"https://www.gravatar.com/avatar/{Convert.ToHexStringLower(hash)}";

		if (options is null)
		{
			return baseUrl;
		}

		var queryParams = new List<string>();

		if (!string.IsNullOrEmpty(options.Default))
		{
			queryParams.Add($"d={Uri.EscapeDataString(options.Default)}");
		}
		if (options.ForceDefault)
		{
			queryParams.Add("f=y");
		}
		if (options.Rating is { } r)
		{
			queryParams.Add($"r={r.ToStringFast().ToLower()}");
		}

		return queryParams.Count > 0
			? $"{baseUrl}?{string.Join("&", queryParams)}"
			: baseUrl;
	}

	[UsedImplicitly]
	public sealed record Options(
		string? Default = null,
		bool ForceDefault = false,
		Ratings? Rating = null
	);

	// ReSharper disable once InconsistentNaming
	[EnumExtensions]
	public enum Ratings
	{
		G,
		PG,
		R,
		X,
	}

	public static class AvatarGenMethods
	{
		public const string None = "404";
		public const string MysteryPerson = "mp";
		public const string Identicon = "identicon";
		public const string MonsterId = "monsterid";
		public const string Wavatar = "wavatar";
		public const string Retro = "retro";
		public const string Robohash = "robohash";
		public const string Blank = "blank";
	}
}