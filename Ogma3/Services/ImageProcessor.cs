using JetBrains.Annotations;
using NetVips;
using Ogma3.Infrastructure.Logging.OperationTiming;

namespace Ogma3.Services;

[RegisterSingleton]
[UsedImplicitly]
public sealed class ImageProcessor(ILogger<ImageProcessor> logger)
{
	private const int MaxFrames = 100;
	public async Task<byte[]> ProcessAvatar(IFormFile file, int? width, int? height, bool allowAnimated)
	{
		if (file is not { Length: > 0 })
		{
			throw new ArgumentException("File cannot be null or empty");
		}

		using var op = logger.TimeOperation("Resizing image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

		await using var stream = file.OpenReadStream();
		var bytes = new byte[stream.Length];
		await stream.ReadExactlyAsync(bytes, 0, (int)stream.Length);

		var w = width ?? height ?? throw new ArgumentException($"At least one of {nameof(width)} and {nameof(height)} should not be null");
		var h = height ?? w;

		return await Task.Run(() => {
			using var processed = Image.ThumbnailBuffer(bytes,
				width: w,
				height: h,
				size: Enums.Size.Down,
				crop: Enums.Interesting.Centre,
				optionString: allowAnimated ? $"[n={MaxFrames}]" : "[n=1]"
			);

			var saveOptions = processed.Contains("n-pages") && (processed.Get("n-pages") as int?) is > 1
				? ".webp[Q=82,strip=true,loop=0]"
				: ".webp[Q=85,strip=true]";

			return processed.WriteToBuffer(saveOptions);
		});
	}

	private const int MaxWidth = 2048;
	public async Task<byte[]> ProcessPaste(IFormFile file)
	{
		if (file is not { Length: > 0 })
		{
			throw new ArgumentException("File cannot be null or empty");
		}

		using var op = logger.TimeOperation("Resizing image {Filename} that weighs {Size} bytes", file.FileName, file.Length);

		await using var stream = file.OpenReadStream();
		var bytes = new byte[stream.Length];
		await stream.ReadExactlyAsync(bytes, 0, (int)stream.Length);

		return await Task.Run(() => {
			using var image = Image.NewFromBuffer(bytes);
			if (image.Width > MaxWidth)
			{
				var scale = (double)MaxWidth / image.Width;
				image.Resize(scale);
			}

			return image.WebpsaveBuffer(q: 80, effort: 4, lossless: false, smartSubsample: true);
		});
	}
}