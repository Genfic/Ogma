using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Exceptions;
using ZstdSharp;

namespace Ogma3.Infrastructure.Compression;

/// <summary>
/// Adds support for <see href="https://github.com/facebook/zstd">Zstandard compression</see>
/// <para/>Implementation based on <see href="https://github.com/rgueldenpfennig/Squidlr/pull/6/files"/>
/// <para/>Native ASP Core implementation tracked by <see href="https://github.com/dotnet/aspnetcore/issues/50643"/>
/// </summary>
/// <param name="options">Options</param>
public sealed class ZstdCompressionProvider(IOptions<ZstdCompressionProvider.Options> options) : ICompressionProvider
{
	private readonly Options _options = options.Value;
	
	/// <inheritdoc />
	public string EncodingName => "zstd";
	
	/// <inheritdoc />
	public bool SupportsFlush => true;

	public Stream CreateStream(Stream outputStream)
	{
		var level = _options.CompressionLevel switch
		{

			CompressionLevel.NoCompression => 0,
			CompressionLevel.Fastest => 1,
			CompressionLevel.Optimal => 5,
			CompressionLevel.SmallestSize => 19,
			_ => throw new UnexpectedEnumValueException<CompressionLevel>(_options.CompressionLevel),
		};
		return new CompressionStream(outputStream, level: level, leaveOpen: true);
	}

	public sealed class Options : IOptions<Options>
	{
		public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Fastest;
		public Options Value => this;
	}
}