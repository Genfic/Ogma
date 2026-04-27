using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ogma3.Areas.Admin.Pages;

public static class NavPages
{
	public static string Index => "Dashboard";
	public static string Settings => "Settings";
	public static string Email => "Mailer";
	public static string Tags => "Tags";
	public static string Quotes => "Quotes";
	public static string InviteCodes => "InviteCodes";
	public static string Documents => "Documents";
	public static string Roles => "Roles";
	public static string Users => "Users";
	public static string ContentBlock => "ContentBlock";
	public static string Ratings => "Ratings";
	public static string ModLog => "ModLog";
	public static string Reports => "Reports";
	public static string Faq => "FAQ";
	public static string Infractions => "Infractions";

	public static string PageNavClass(ViewContext viewContext, string page)
	{
		var activePage = viewContext.ViewData["ActivePage"] as string
		                 ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

		return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : "";
	}
}