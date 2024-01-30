using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public class DownloadPersonalDataModel(
	UserManager<OgmaUser> userManager,
	ILogger<DownloadPersonalDataModel> logger)
	: PageModel
{
	private static IEnumerable<PropertyInfo>? _personalProperties;
	
	public async Task<IActionResult> OnPostAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		logger.LogInformation("User with ID '{UserId}' asked for their personal data", userManager.GetUserId(User));

		// Only include personal data for download
		// TODO: Write a source generator to handle this
		_personalProperties ??= typeof(OgmaUser)
			.GetProperties()
			.Where(prop => !prop.Name.Contains("phone", StringComparison.CurrentCultureIgnoreCase))
			.Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

		var personalData = _personalProperties
			.ToDictionary(p => p.Name, p => p.GetValue(user)?.ToString() ?? "null");

		var json = JsonSerializer.Serialize(personalData, PersonalDataDictionaryJsonContext.Default.DictionaryStringString);
		
		Response.Headers.ContentDisposition = "attachment; filename=PersonalData.json";
		return new FileContentResult(Encoding.UTF8.GetBytes(json), "text/json");
	}
}

[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class PersonalDataDictionaryJsonContext : JsonSerializerContext;