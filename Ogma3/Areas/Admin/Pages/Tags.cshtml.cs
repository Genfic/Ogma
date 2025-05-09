using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Areas.Admin.Pages;

public sealed class Tags : PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public required string StaticDataJson { get; set; }

	public sealed class InputModel
	{
		[Required] public required string Name { get; init; }
		[Required] public required string Description { get; init; }

		public ETagNamespace Namespace { get; init; }
	}

	public void OnGet()
	{
		var data = new StaticData(
			new ValidationData(
				CTConfig.Tag.MinNameLength,
				CTConfig.Tag.MaxNameLength,
				CTConfig.Tag.MaxDescLength
			)
		);
		StaticDataJson = JsonSerializer.Serialize(data, StaticDataJsonContext.Default.StaticData);
	}
}

public sealed record StaticData(ValidationData Validation);

public sealed record ValidationData(int MinNameLength, int MaxNameLength, int MaxDescLength);

[JsonSerializable(typeof(StaticData))]
public sealed partial class StaticDataJsonContext : JsonSerializerContext;