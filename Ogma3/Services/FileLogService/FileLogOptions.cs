namespace Ogma3.Services.FileLogService;

public sealed class FileLogOptions
{
	public required string Directory { get; set; }
	public int MaxSizeInBytes { get; set; } = 5 * 1024 * 1024;
	public int CompressionLevel { get; set; } = 3;
}