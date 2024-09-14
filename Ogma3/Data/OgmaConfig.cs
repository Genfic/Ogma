#nullable disable

using System.Text.Json;
using System.Text.Json.Serialization;
using Ogma3.Infrastructure.Attributes;

namespace Ogma3.Data;

public sealed class OgmaConfig
{
	[JsonIgnore] private string _persistentFileLocation = string.Empty;
	
	/// <summary>
	/// Persist `this` in a file asynchronously
	/// </summary>
	public async Task PersistAsync()
	{
		await using var sw = new StreamWriter(_persistentFileLocation);
		var json = JsonSerializer.Serialize(this, OgmaConfigJsonContext.Default.OgmaConfig);
		await sw.WriteAsync(json);
	}
	
	/// <summary>
	/// Initialize the config with data
	/// </summary>
	/// <param name="persistentFileLocation">Location of the file to persist the data to</param>
	/// <returns></returns>
	public static OgmaConfig Init(string persistentFileLocation)
	{
		using var sr = new StreamReader(persistentFileLocation);
		var config = JsonSerializer.Deserialize(sr.ReadToEnd(), OgmaConfigJsonContext.Default.OgmaConfig) ?? new OgmaConfig();
		config._persistentFileLocation = persistentFileLocation;
		return config;
	}
	
	public string Cdn { get; set; }
	
	public string AvatarServiceUrl { get; set; }
	public string AdminEmail { get; set; }
	public int MaxInvitesPerUser { get; set; }
	
	// Docs
	[AutoformCategory("Docs")] public string PrivacyPolicyDoc { get; set; }
	[AutoformCategory("Docs")] public string AboutDoc { get; set; }
	
	// Pagination settings
	[AutoformCategory("Pagination")] public int CommentsPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int ClubsPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int BlogpostsPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int StoriesPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int ClubThreadsPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int ShelvesPerPage { get; set; } = 50;
	
	// Image sizes
	[AutoformCategory("Image sizes")] public int StoryCoverWidth { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int StoryCoverHeight { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int ClubIconWidth { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int ClubIconHeight { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int AvatarWidth { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int AvatarHeight { get; set; } = 250;
	
	[AutoformCategory("Moderation")] public int MinReportLength { get; set; } = 30;
}

[JsonSerializable(typeof(OgmaConfig))]
[JsonSourceGenerationOptions(WriteIndented = true, ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true)]
public sealed partial class OgmaConfigJsonContext : JsonSerializerContext;