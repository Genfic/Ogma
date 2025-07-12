namespace Ogma3.Services;

public sealed class WwwrootService(IWebHostEnvironment host)
{
	public async Task SaveFile(string path, string filename, byte[] content)
	{
		var dir = Path.Combine(host.WebRootPath, path);
		EnsureDirectory(dir);
		var dest = Path.Combine(dir, filename);
		await File.WriteAllBytesAsync(dest, content);
	}

	public async Task<byte[]> ReadFile(string path, string filename)
	{
		var dir = Path.Combine(host.WebRootPath, path, filename);
		return await File.ReadAllBytesAsync(dir);
	}

	public void DeleteFile(string path, string filename)
	{
		var dir = Path.Combine(host.WebRootPath, path, filename);
		File.Delete(dir);
	}

	private static void EnsureDirectory(string directory)
	{
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
	}
}