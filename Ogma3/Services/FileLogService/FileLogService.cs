using System.Text;
using Microsoft.Extensions.Options;
using ZstdSharp;

namespace Ogma3.Services.FileLogService;

public sealed class FileLogService(IOptions<FileLogOptions> options) : IFileLogService
{
	public string FileName { get; set; } = "log";

	private string LogPath => Path.Combine(options.Value.Directory, $"{FileName}.txt");
	private string ArchivePath => Path.Combine(options.Value.Directory, $"{FileName}.{DateTime.UtcNow:yyyyMMddHHmmss}.zstd");

	public async Task Write(string message, CancellationToken cancellationToken = default)
	{
		if (!Directory.Exists(options.Value.Directory))
		{
			Directory.CreateDirectory(options.Value.Directory);
		}

		await File.AppendAllTextAsync(LogPath, message + Environment.NewLine, Encoding.UTF8, cancellationToken);
		await RollOverIfNeeded();
	}

	public IEnumerable<string> ReadLines()
	{
		using var fs = new FileStream(LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		using var reader = new StreamReader(fs, Encoding.UTF8);
		while (reader.ReadLine() is {} line)
		{
			yield return line;
		}
	}

	public string ReadAll()
	{
		using var fs = new FileStream(LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		using var reader = new StreamReader(fs, Encoding.UTF8);
		return reader.ReadToEnd();
	}

	private async Task RollOverIfNeeded()
	{
		var fileInfo = new FileInfo(LogPath);
		if (fileInfo.Length <= options.Value.MaxSizeInBytes) return;

		await using var source = File.Open(LogPath, FileMode.Open, FileAccess.ReadWrite);
		await using var archive = File.OpenWrite(ArchivePath);
		await using var compression = new CompressionStream(archive, options.Value.CompressionLevel);
		await source.CopyToAsync(compression);
		source.SetLength(0);
		source.Close();
	}
}