using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using Serilog;
using Utils.Extensions;

namespace Ogma3.Infrastructure.Middleware;

public static class CustomStaticFiles
{
	private static readonly Dictionary<string, string> CustomMimeTypes = new()
	{
		[".epub"] = "application/epub+zip",
	};

	public static WebApplication UseCustomStaticFiles(this WebApplication app)
	{
		var extensionsProvider = new FileExtensionContentTypeProvider();
		extensionsProvider.Mappings.AddMany(CustomMimeTypes);

		app.UseStaticFiles(new StaticFileOptions
		{
			HttpsCompression = HttpsCompressionMode.Compress,
			OnPrepareResponse = PrepareResponse,
			ContentTypeProvider = extensionsProvider,
		});
		return app;
	}

	private static void PrepareResponse(StaticFileResponseContext context)
	{
		context.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
		{
			Public = true,
			MaxAge = TimeSpan.FromDays(365),
		};

		if (context.File.Name.Contains("worker"))
		{
			Log.Information("Serving a service worker");
			context.Context.Response.Headers.Append("Service-Worker-Allowed", "/");
		}
	}
}