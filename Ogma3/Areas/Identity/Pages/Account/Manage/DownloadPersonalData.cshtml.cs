#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Json;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public class DownloadPersonalDataModel : PageModel
{
	private readonly UserManager<OgmaUser> _userManager;
	private readonly ILogger<DownloadPersonalDataModel> _logger;

	public DownloadPersonalDataModel(
		UserManager<OgmaUser> userManager,
		ILogger<DownloadPersonalDataModel> logger)
	{
		_userManager = userManager;
		_logger = logger;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		_logger.LogInformation("User with ID '{UserId}' asked for their personal data", _userManager.GetUserId(User));

		// Only include personal data for download
		var personalDataProps = typeof(OgmaUser)
			.GetProperties()
			.Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

		var personalData = personalDataProps
			.Where(p => !p.Name.Contains("phone", StringComparison.CurrentCultureIgnoreCase))
			.ToDictionary(p => p.Name, p => p.GetValue(user)?.ToString() ?? "null");


		Response.Headers.ContentDisposition = "attachment; filename=PersonalData.json";
		return new FileContentResult(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(personalData, SerializerOptions.Indented)), "text/json");
	}
}