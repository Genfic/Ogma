using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Services;
using Ogma3.Services.FileUploader;

namespace Ogma3.Api.V1.Files;

[Handler]
[MapPost("api/files/upload")]
[Authorize]
public sealed partial class UploadFile(IFileUploader uploader, ImageProcessor processor, OgmaConfig config)
{

	private async ValueTask<Ok<string>> Handle(
		[FromForm] Query request,
		CancellationToken _
	)
	{
		var processed = await processor.ProcessPaste(request.File);
		var res = await uploader.Upload(processed, "paste");

		return TypedResults.Ok(Path.Join(config.Cdn, res.Key));
	}

	public sealed record Query(IFormFile File);
}