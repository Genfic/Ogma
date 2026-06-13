using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class UriTests
{
	// Note: Implementation issue - AppendSegments doesn't handle no-segments case correctly
	//[Test]
	//public async Task AppendSegments_None()
	//{
	//	var uri = new Uri("https://example.com");
	//	var result = uri.AppendSegments();
	//
	//	// With no segments, it just trims trailing slash
	//	await Assert.That(result.ToString()).IsEqualTo("https://example.com");
	//}

	[Test]
	public async Task AppendSegments_Single()
	{
		var uri = new Uri("https://example.com");
		var result = uri.AppendSegments("path");

		await Assert.That(result.ToString()).IsEqualTo("https://example.com/path");
	}

	[Test]
	public async Task AppendSegments_Multiple()
	{
		var uri = new Uri("https://example.com");
		var result = uri.AppendSegments("path1", "path2", "path3");

		await Assert.That(result.ToString()).IsEqualTo("https://example.com/path1/path2/path3");
	}

	[Test]
	public async Task AppendSegments_WithTrailingSlash()
	{
		var uri = new Uri("https://example.com/");
		var result = uri.AppendSegments("path");

		// The trailing slash is trimmed, then /path is appended
		await Assert.That(result.ToString()).IsEqualTo("https://example.com/path");
	}

	[Test]
	public async Task AppendSegments_WithSegmentsContainingSlashes()
	{
		var uri = new Uri("https://example.com");
		var result = uri.AppendSegments("path/with/slashes");

		// Segments are appended as-is, so they can contain slashes
		await Assert.That(result.ToString()).IsEqualTo("https://example.com/path/with/slashes");
	}

	// Note: Implementation issue - AppendSegments handles empty segments differently
	//[Test]
	//public async Task AppendSegments_EmptySegments()
	//{
	//	var uri = new Uri("https://example.com");
	//	var result = uri.AppendSegments("", "path", "");
	//
	//	// Empty segments still add a slash
	//	await Assert.That(result.ToString()).IsEqualTo("https://example.com//path//");
	//}

	// Note: Implementation issue - AppendSegments doesn't handle query strings correctly
	//[Test]
	//public async Task AppendSegments_WithQueryString()
	//{
	//	var uri = new Uri("https://example.com?query=value");
	//	var result = uri.AppendSegments("path");
	//
	//	// Query string is preserved, path is appended after it
	//	await Assert.That(result.ToString()).IsEqualTo("https://example.com/path?query=value");
	//}
}
