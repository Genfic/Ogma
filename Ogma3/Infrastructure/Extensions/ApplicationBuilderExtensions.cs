using Microsoft.AspNetCore.Rewrite;

namespace Ogma3.Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseRewriter(this IApplicationBuilder builder, Action<RewriteOptions> configure)
	{
		var options = new RewriteOptions();
		configure(options);
		return builder.UseRewriter(options);
	}
}