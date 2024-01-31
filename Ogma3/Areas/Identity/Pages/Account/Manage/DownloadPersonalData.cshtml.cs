using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public class DownloadPersonalDataModel(
	ApplicationDbContext context,
	ILogger<DownloadPersonalDataModel> logger)
	: PageModel
{
	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not { } uid) return Unauthorized();
		
		var user = await context.Users
			.Where(u => u.Id == uid)
			.ProjectToDto()
			.FirstOrDefaultAsync();
		
		if (user is null)
		{
			return NotFound($"Unable to load user with ID {uid}.");
		}

		logger.LogInformation("User with ID {UserId} asked for their personal data", uid);

		var json = JsonSerializer.Serialize(user, UserPersonalDataJsonContext.Default.UserPersonalData);
		
		Response.Headers.ContentDisposition = "attachment; filename=PersonalData.json";
		return new FileContentResult(Encoding.UTF8.GetBytes(json), "text/json");
	}
}

[JsonSerializable(typeof(UserPersonalData))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class UserPersonalDataJsonContext : JsonSerializerContext;