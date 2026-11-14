using Immediate.Injections.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Services.Security;

/// <summary>
/// Helpers for mutating endpoints and pages to require fresh account proof through a
/// security transaction. Produces a stable machine-readable <c>reauthentication_required</c>
/// problem details result for API endpoints and redirects for Razor pages.
/// </summary>
public static class ReauthRequired
{
	public const string Title = "reauthentication_required";
	public const int StatusCode = StatusCodes.Status428PreconditionRequired;

	/// <summary>
	/// Creates (or reuses) a pending security transaction for the operation and returns a
	/// problem details result telling the client where to complete reauthentication.
	/// </summary>
	public static ProblemHttpResult Problem(SecurityTransaction transaction, LinkGenerator linkGenerator, HttpContext http)
	{
		var reauthenticationUrl = linkGenerator.GetPathByPage(
			http,
			page: "/Account/Reauthenticate",
			values: new { area = "Identity", transaction = transaction.Id });

		return TypedResults.Problem(
			title: Title,
			statusCode: StatusCode,
			detail: "This change requires fresh verification of your credentials.",
			extensions: new Dictionary<string, object?>
			{
				["transactionId"] = transaction.Id,
				["reauthenticationUrl"] = reauthenticationUrl,
			});
	}

	/// <summary>
	/// Redirects the current POST to the reauthentication page, preserving the pending
	/// operation and a validated return destination.
	/// </summary>
	public static RedirectToPageResult RedirectToReauth(SecurityTransaction transaction, string returnUrl, PageModel page)
		=> Account_Reauthenticate.Get(transaction.Id, returnUrl).Redirect(page);
}
