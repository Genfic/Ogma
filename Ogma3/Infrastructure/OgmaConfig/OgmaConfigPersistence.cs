using System.Text.Json;

namespace Ogma3.Infrastructure.OgmaConfig;

public sealed class OgmaConfigPersistence
{
	public OgmaConfig Config { get; }
	private readonly string _path;

	private OgmaConfigPersistence(OgmaConfig config, string path)
	{
		Config = config;
		_path = path;
	}

	/// <summary>
	/// Asynchronously initializes an instance of <see cref="OgmaConfigPersistence"/> by loading
	/// configuration data from a specified file location. If the specified file does not exist,
	/// a new default configuration instance will be used.
	/// </summary>
	/// <param name="fileLocation">The relative path to the configuration file.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains an instance of <see cref="OgmaConfigPersistence"/>.</returns>
	public static async Task<OgmaConfigPersistence> InitAsync(string fileLocation)
	{
		var path = Path.Combine(AppContext.BaseDirectory, fileLocation);

		if (!File.Exists(path))
		{
			return new OgmaConfigPersistence(new(), path);
		}

		await using var fs = File.OpenRead(path);
		var config = await JsonSerializer.DeserializeAsync<OgmaConfig>(fs, OgmaConfigJsonContext.Default.OgmaConfig) ?? new();

		return new OgmaConfigPersistence(config, path);
	}

	/// <summary>
	/// Asynchronously persists the current instance of <see cref="OgmaConfig"/> to the specified file path.
	/// A temporary file is used to ensure atomicity of the save operation, minimizing the risk of data corruption
	/// in case of an interruption during the writing process.
	/// </summary>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public async Task PersistAsync()
	{
		var tmpPath = _path + ".tmp";

		await using (var fs = File.Create(tmpPath))
		{
			await JsonSerializer.SerializeAsync(fs, Config, OgmaConfigJsonContext.Default.OgmaConfig);
			await fs.FlushAsync();
		}

		File.Replace(tmpPath, _path, null);
	}
}