using System.Security.Claims;

namespace Ogma3.Infrastructure.Extensions;

public static class ClaimTypesExtensions
{
	extension(ClaimTypes)
	{
		public static string Avatar => "x:Avatar";
		public static string Title => "x:Title";
		public static string IsStaff => "x:IsStaff";
	}
}