using Utils.Extensions;

namespace Ogma3.Data.Constants;

public static class SystemUserConstants
{
	public static readonly SystemUser Deleted = new(-1, "Deleted User");
	public static readonly SystemUser Anonymous = new(-2, "Anonymous User");

	public readonly record struct SystemUser(long Id, string Name)
	{
		public string NormalizedName => Name.Normalize().ToUpperInvariant();
		public string Avatar => $"/img/placeholders/{Name.Friendlify().ToLowerInvariant()}.png";
	}
}