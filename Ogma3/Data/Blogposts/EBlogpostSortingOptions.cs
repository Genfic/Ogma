using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ogma3.Data.Blogposts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EBlogpostSortingOptions
{
	[Display(Name = "Title ↑")] TitleAscending,
	[Display(Name = "Title ↓")] TitleDescending,

	[Display(Name = "Date ↑")] DateAscending,
	[Display(Name = "Date ↓")] DateDescending,

	[Display(Name = "Words ↑")] WordsAscending,
	[Display(Name = "Words ↓")] WordsDescending,
}