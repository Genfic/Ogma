using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileProviders;
using Serilog;

namespace Ogma3.Infrastructure.StartupGenerators;

public class JavascriptFilesManifestGenerator(IWebHostEnvironment environment)
{
	private const string Root = "./wwwroot";
	
	private readonly IFileProvider _fileProvider = environment.WebRootFileProvider;

	private sealed record Manifest(DateTime GeneratedAt, Dictionary<string, string> Files);

	public void Generate(params string[] directories)
	{
		Log.Information("Preparing JS manifest");
		
		Dictionary<string, string> filesAndHashes = new();

		var files = new List<string>();

		foreach (var directory in directories)
		{
			files.AddRange(Directory.GetFiles(Path.Join(Root, directory.Replace(Root, "")), "*.js", SearchOption.AllDirectories));
		}
		
		if (files.Count <= 0)
		{
			Log.Information("\t📃 No files found");
			return;
		}
		
		foreach (var file in files)
		{
			var subpath = file.Replace(Root, "").Replace('\\', '/');
			var fileInfo = _fileProvider.GetFileInfo(subpath);
			if (fileInfo.Exists)
			{
				var hash = GetHashForFile(fileInfo);
				filesAndHashes.Add(subpath, hash);
				Log.Verbose("\t📃 File {FileName} was found with hash {Hash}", subpath, hash);
			}
			else
			{
				Log.Verbose("\t📃 File {Filename} does not exist", file);
			}
		}

		var manifest = JsonSerializer.Serialize(new Manifest(DateTime.UtcNow, filesAndHashes));
		File.WriteAllText(Path.Join(Root, "manifest.js.json"), manifest);
		
		Log.Information("Manifest ready. {FilesFound} files out of {AllFiles} were found.", filesAndHashes.Count, files.Count);
	}
	
	private static string GetHashForFile(IFileInfo fileInfo)
	{
		using var readStream = fileInfo.CreateReadStream();
		var hash = SHA256.HashData(readStream);
		return WebEncoders.Base64UrlEncode(hash);
	}
}