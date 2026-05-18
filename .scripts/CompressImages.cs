#:package NetVips@3.2.0
#:package NetVips.Native@8.18.2
#:package Glob@1.1.9

using System.Diagnostics;
using NetVips;
using GlobExpressions;

var source = args[0];
var glob = args[1];
var target = args[2];
var verbose = args.Contains("--verbose");

var start = Stopwatch.StartNew();

Console.WriteLine($"Processing images ({Path.Join(source, glob)})");

var tasks = Glob.Files(source, glob)
	.Select(async path => {

		Stopwatch? subTimer = null;
		if (verbose)
		{
			subTimer =  Stopwatch.StartNew();
			Console.WriteLine($"Processing {path}");
		}

		var name = Path.GetFileNameWithoutExtension(path);
		var webpOut = Path.Combine(target, $"{name}.webp");
		var avifOut = Path.Combine(target, $"{name}.avif");

		var file = await File.ReadAllBytesAsync(Path.Combine(source, path));
		var (webp, avif) = await Task.Run(() => {
			using var img = Image.NewFromBuffer(file);

			var webp = img.WriteToBuffer(".webp[Q=85,strip=true]");
			var avif = img.WriteToBuffer(".avif[Q=85,strip=true]");

			return (webp, avif);
		});

		await File.WriteAllBytesAsync(webpOut, webp);
		await File.WriteAllBytesAsync(avifOut, avif);

		if (verbose)
		{
			Console.WriteLine($"Processed {path} in {subTimer?.ElapsedMilliseconds}ms");
		}
	})
	.ToArray();

await Task.WhenAll(tasks);

Console.WriteLine($"Processed {tasks.Length} images in {start.ElapsedMilliseconds}ms");