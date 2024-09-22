using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileProviders;
using Serilog;

namespace Ogma3.Infrastructure.StartupGenerators;

public sealed class JavascriptFilesManifestGenerator(IWebHostEnvironment environment)
{
	private const string Root = "./wwwroot";
	
	private readonly IFileProvider _fileProvider = environment.WebRootFileProvider;
	private readonly string _manifestPath = Path.Join(Root, "manifest.js.json");
	
	public void Generate(params string[] directories)
	{
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		
		// We're using static logging here because the generator is not a part of the DI container and thus, we cannot inject it
		Log.Information("Preparing JS manifest");

		var existingManifest = File.Exists(_manifestPath)
			? JsonSerializer.Deserialize(File.ReadAllText(_manifestPath), ManifestJsonContext.Default.Manifest)
			: null;
		
		ConcurrentDictionary<string, string> filesAndHashesConcurrent = new();

		var files = directories
			.SelectMany(directory => Directory.GetFiles(Path.Join(Root, directory.Replace(Root, "")), "*.js", SearchOption.AllDirectories))
			.Select(file => file.Replace(Root, "").Replace('\\', '/'))
			.ToImmutableList();
		
		if (files.Count <= 0)
		{
			Log.Information("\t📃 No files found");
			return;
		}

		_ = Parallel.ForEach(files, file =>
		{
			var fileInfo = _fileProvider.GetFileInfo(file);
			if (fileInfo.Exists)
			{
				var hash = GetHashForFile(fileInfo);
				_ = filesAndHashesConcurrent.TryAdd(file, hash);
				Log.Verbose("\t📃 File {FileName} was found with hash {Hash}", file, hash);
			}
			else
			{
				Log.Information("\t📃 File {Filename} does not exist", file);
			}
		});

		var filesAndHashes = filesAndHashesConcurrent.ToImmutableSortedDictionary(new AlphaComparer());
		
		if (existingManifest is {} em && filesAndHashes.SequenceEqual(em.Files.ToImmutableSortedDictionary(new AlphaComparer()), new KvpComparer()))
		{
			stopwatch.Stop();
			Log.Information("Files are unchanged, stopping manifest generation after {Time}ms", stopwatch.ElapsedMilliseconds);
			return;
		}
		
		var manifest = JsonSerializer.Serialize(new Manifest(DateTimeOffset.UtcNow, filesAndHashes), ManifestJsonContext.Default.Manifest);
		File.WriteAllText(_manifestPath, manifest);
		
		stopwatch.Stop();
		Log.Information("Manifest ready ({Time} ms). {FilesFound} files out of {AllFiles} were found.", stopwatch.ElapsedMilliseconds, filesAndHashesConcurrent.Count, files.Count);
		if (files.Except(filesAndHashesConcurrent.Keys).Any())
		{
			Log.Information("Files missing from the manifest: {Files}", files.Except(filesAndHashesConcurrent.Keys));
		}
	}
	
	private static string GetHashForFile(IFileInfo fileInfo)
	{
		using var readStream = fileInfo.CreateReadStream();
		var hash = SHA256.HashData(readStream);
		return WebEncoders.Base64UrlEncode(hash);
	}
}

[UsedImplicitly]
internal sealed record Manifest(DateTimeOffset GeneratedAt, ImmutableSortedDictionary<string, string> Files);


[JsonSerializable(typeof(Manifest))]
[JsonSourceGenerationOptions(WriteIndented = true)]
internal sealed partial class ManifestJsonContext : JsonSerializerContext;


sealed file class AlphaComparer : IComparer<string>
{
	public int Compare(string? x, string? y) 
		=> string.CompareOrdinal(x, y);
}


sealed file class KvpComparer : IEqualityComparer<KeyValuePair<string, string>>
{
	public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
		=> x.Key == y.Key && x.Value == y.Value;
	
	public int GetHashCode(KeyValuePair<string, string> obj)
		=> HashCode.Combine(obj.Key, obj.Value);
}