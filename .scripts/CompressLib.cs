#! usr/bin/env dotnet
#:package Glob@1.1.9

using System.Diagnostics;
using System.IO.Compression;
using GlobExpressions;

var source = args[0];
var verbose = args.Contains("--verbose");

var start = Stopwatch.StartNew();

var files = Glob.Files(source, "**/*.{js,css}").ToArray();

Parallel.ForEach(
	files,
	(path) =>
	{
		Stopwatch? subTimer = null;
		if (verbose)
		{
			subTimer = Stopwatch.StartNew();
			Console.WriteLine($"Processing {path}");
		}

		var dir = Path.GetDirectoryName(path) ?? "";
		var name = Path.GetFileName(path);

		using var sourceStream = File.OpenRead(Path.Combine(source, path));
		Compress(sourceStream, Path.Combine(source, dir, $"{name}.gz"), s => new GZipStream(s, CompressionLevel.Optimal));
		Compress(sourceStream, Path.Combine(source, dir, $"{name}.br"), s => new BrotliStream(s, CompressionLevel.Optimal));
		Compress(sourceStream, Path.Combine(source, dir, $"{name}.zst"), s => new ZstandardStream(s, CompressionLevel.Optimal));

		if (verbose)
		{
			Console.WriteLine($"Processed {path} in {subTimer?.ElapsedMilliseconds}ms");
		}
	}
);

Console.WriteLine($"Processed {files.Length} files in {start.ElapsedMilliseconds}ms");

return;

static void Compress(Stream source, string destination, Func<Stream, Stream> compressor)
{
	using var destStream = File.Create(destination);
	using var compressionStream = compressor(destStream);
	source.CopyTo(compressionStream);
	source.Position = 0;
}
