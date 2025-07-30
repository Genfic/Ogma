// Compile-time config variables

namespace Ogma3.Data;

// ReSharper disable once InconsistentNaming
public static class CTConfig
{
	/// <summary>
	/// Technically, maximum length is 256 according to <see href="https://www.rfc-editor.org/errata_search.php?rfc=3696&eid=1690">RFC 3696 Errata</see>
	/// However, two characters of a valid path are angle brackets, which leaves us with 254
	/// </summary>
	public const int MaxEmailAddressLength = 254;

	public static class Files
	{
		public const int AvatarMaxWeight = 1 * 1024 * 1024;
	}

	public static class User
	{
		public const int MinNameLength = 5;
		public const int MaxNameLength = 20;

		public const int MinPassLength = 10;
		public const int MaxPassLength = 200;

		public const int MaxTitleLength = 20;
		public const int MaxBioLength = 10_000;
		public const int MaxLinksAmount = 5;
	}

	public static class Tag
	{
		public const int MinNameLength = 3;
		public const int MaxNameLength = 25;

		public const int MaxDescLength = 250;
	}

	public static class Rating
	{
		public const int MinNameLength = 4;
		public const int MaxNameLength = 20;

		public const int MinDescriptionLength = 5;
		public const int MaxDescriptionLength = 1000;
	}

	public static class Chapter
	{
		public const int MinTitleLength = 5;
		public const int MaxTitleLength = 100;

		public const int MinBodyLength = 5_000;
		public const int MaxBodyLength = 500_000;

		public const int MaxNotesLength = 500;

		public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";
		public const string ValidateNoteLengthMsg = "The {0} cannot exceed {1} characters.";
	}

	public static class Story
	{
		public const int MinTitleLength = 3;
		public const int MaxTitleLength = 100;

		public const int MinDescriptionLength = 100;
		public const int MaxDescriptionLength = 3000;

		public const int MinHookLength = 50;
		public const int MaxHookLength = 250;
		public const int CoverMaxWeight = 2 * 1024 * 1024;

		public const int MinTagCount = 3;

		public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";
		public const string ValidateFileWeight = "The {0} must be less than {1} bytes";
		public const string ValidateTagCount = "You must select {0} tags or more";
	}

	public static class Comment
	{
		public const int MinBodyLength = 2;
		public const int MaxBodyLength = 5000;
	}

	public static class Shelf
	{
		public const int MinNameLength = 3;
		public const int MaxNameLength = 20;

		public const int MaxDescriptionLength = 100;
	}

	public static class Blogpost
	{
		public const int MinTitleLength = 3;
		public const int MaxTitleLength = 100;

		public const int MinBodyLength = 50;
		public const int MaxBodyLength = 500_000;

		public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";

		public const int MaxTagsAmount = 10;
		public const int MaxTagLength = 20;
		public const string ValidateTagsCountMsg = "The {0} can have a maximum of {1} tags.";
	}

	public static class Club
	{
		public const int MinNameLength = 3;
		public const int MaxNameLength = 50;

		public const int MinHookLength = 10;
		public const int MaxHookLength = 100;

		public const int MaxDescriptionLength = 25_000;

		public const int CoverMaxWeight = 2 * 1024 * 1024;

		public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";
		public const string ValidateFileWeight = "The {0} must be less than {1} bytes";
	}

	public static class ClubThread
	{
		public const int MinTitleLength = 3;
		public const int MaxTitleLength = 100;

		public const int MinBodyLength = 20;
		public const int MaxBodyLength = 25_000;
	}

	public static class Folder
	{
		public const int MinNameLength = 3;
		public const int MaxNameLength = 20;

		public const int MaxDescriptionLength = 500;

		public const string ValidateLengthMsg = "The {0} must be at least {2} and at most {1} characters long.";
	}

	public static class ClubThreadComment
	{
		public const int MinBodyLength = 3;
		public const int MaxBodyLength = 5000;
	}

	public static class Report
	{
		public const int MinReasonLength = 30;
		public const int MaxReasonLength = 500;
	}
}