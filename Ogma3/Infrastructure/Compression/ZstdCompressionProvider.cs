using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;

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

	/// <inheritdoc />
	public Stream CreateStream(Stream outputStream)
	{
		return new ZstandardStream(outputStream, _options.CompressionOptions, leaveOpen: true);
	}

	public sealed class Options : IOptions<Options>
	{
		/// <summary>
		/// The compression options to use for the stream.
		/// </summary>
		public ZstandardCompressionOptions CompressionOptions { get; set; } = new();

		/// <inheritdoc />
		Options IOptions<Options>.Value => this;
	}
}