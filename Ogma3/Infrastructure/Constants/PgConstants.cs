namespace Ogma3.Infrastructure.Constants;

public static class PgConstants
{
	public const string CurrentTimestamp = "CURRENT_TIMESTAMP";
	public const string DuplicatePrimaryKeyError = "23505";
	public const string EmptyArray = "'{}'";

	public static class CollationNames
	{
		public const string CaseInsensitive = "nocase";
		public const string CaseInsensitiveNoAccent = "nocase-noaccent";
	}
}