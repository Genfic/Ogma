using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ogma3.Areas.Admin.Pages
{
    public class ManageNavPages
    {
        public static string Index => "Index";
        public static string Settings => "Settings";
        public static string Tags => "Tags";
        public static string Namespaces => "Namespaces";
        public static string Quotes => "Quotes";
        public static string InviteCodes => "InviteCodes";
        public static string Documents => "ManageDocuments";
        public static string Roles => "Roles";
        public static string Ratings => "Ratings";


        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        public static string SettingsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Settings);
        public static string TagsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Tags);
        public static string NamespacesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Namespaces);
        public static string QuotesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Quotes);
        public static string InviteCodesNavClass(ViewContext viewContext) => PageNavClass(viewContext, InviteCodes);
        public static string DocumentsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Documents);
        public static string RolesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Roles);
        public static string RatingsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Ratings);
        
        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}