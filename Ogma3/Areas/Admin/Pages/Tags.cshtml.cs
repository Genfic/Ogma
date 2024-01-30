using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Api.V1.Tags;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Areas.Admin.Pages;

public class Tags : PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public required string StaticDataJson { get; set; }

	public class InputModel
	{
		[Required] public required string Name { get; init; }
		[Required] public required string Description { get; init; }

		public ETagNamespace Namespace { get; init; }
	}

	public void OnGet()
	{
		var data = new StaticData(
			Url.RouteUrl(nameof(TagsController)),
			new ValidationData(
				CTConfig.CTag.MinNameLength,
				CTConfig.CTag.MaxNameLength,
				CTConfig.CTag.MaxDescLength
			)
		);
		StaticDataJson = JsonSerializer.Serialize(data, StaticDataJsonContext.Default.StaticData);
	}
}

public record StaticData(string? Route, ValidationData Validation);

public record ValidationData(int MinNameLength, int MaxNameLength, int MaxDescLength);

[JsonSerializable(typeof(StaticData))]
public partial class StaticDataJsonContext : JsonSerializerContext;