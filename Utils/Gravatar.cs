#nullable enable

#region

using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Flurl;

#endregion

namespace Utils;

public static class Gravatar
{
	public static string Generate(string email, Options? options = null)
	{
		const string url = "https://www.gravatar.com/avatar";

		using var md5 = MD5.Create();
		var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower()));
		var emailHash = string.Join("", hash.Select(x => x.ToString("x2")));

		var avatar = new Url(url).AppendPathSegment(emailHash);

		if (options is null) return avatar;

		if (!string.IsNullOrEmpty(options.Default)) avatar = avatar.SetQueryParam("d", options.Default);
		if (options.ForceDefault) avatar = avatar.SetQueryParam("f", "y");
		if (options.Rating is { } r) avatar = avatar.SetQueryParam("r", r.ToString().ToLower());

		return avatar;
	}

	public sealed record Options(
		string? Default = null,
		bool ForceDefault = false,
		Ratings? Rating = null
	);

	// ReSharper disable once InconsistentNaming
	public enum Ratings
	{
		G,
		PG,
		R,
		X
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