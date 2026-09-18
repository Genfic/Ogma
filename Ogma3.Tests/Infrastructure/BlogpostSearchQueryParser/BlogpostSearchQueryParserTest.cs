using Ogma3.Infrastructure.BlogpostSearchQueryParser;
using Parser = Ogma3.Infrastructure.BlogpostSearchQueryParser.BlogpostSearchQueryParser;

namespace Ogma3.Tests.Infrastructure.BlogpostSearchQueryParser;

public sealed class BlogpostSearchQueryParserTest
{
	private static IReadOnlyList<BlogpostSearchToken> Parse(string query)
		=> Parser.Parse(query);

	[Test]
	[Arguments("")]
	[Arguments(" ")]
	[Arguments(",")]
	[Arguments(" , ")]
	public async Task TestParseEmpty(string query)
	{
		await Assert.That(Parse(query)).IsEmpty();
	}

	[Test]
	public async Task TestParseSingleTag()
	{
		var tokens = Parse("dragons");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostTagToken("dragons"));
	}

	[Test]
	public async Task TestParseTagTrimsWhitespace()
	{
		var tokens = Parse("  dragons  ");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostTagToken("dragons"));
	}

	[Test]
	public async Task TestParseMultipleTags()
	{
		var tokens = Parse(" dragons , magic ");
		await Assert.That(tokens.Count).IsEqualTo(2);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostTagToken("dragons"));
		await Assert.That(tokens[1]).IsEqualTo(new BlogpostTagToken("magic"));
	}

	[Test]
	public async Task TestParseNegatedTag()
	{
		var tokens = Parse("-dragons");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostTagToken("dragons", true));
	}

	[Test]
	public async Task TestParseTitle()
	{
		var tokens = Parse("\"a dragon's tale\"");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostTitleToken("a dragon's tale"));
	}

	[Test]
	public async Task TestParseTitleTrimsWhitespace()
	{
		var tokens = Parse("\"  spaced  \"");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostTitleToken("spaced"));
	}

	[Test]
	public async Task TestParseCommaInsideQuotesIsNotASeparator()
	{
		var tokens = Parse("\"a, b\", tag");
		await Assert.That(tokens.Count).IsEqualTo(2);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostTitleToken("a, b"));
		await Assert.That(tokens[1]).IsEqualTo(new BlogpostTagToken("tag"));
	}

	[Test]
	public async Task TestParseAuthor()
	{
		var tokens = Parse("author:maras");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostAuthorToken("maras"));
	}

	[Test]
	public async Task TestParseAuthorTrimsWhitespace()
	{
		var tokens = Parse("author:  maras  ");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostAuthorToken("maras"));
	}

	[Test]
	public async Task TestParseNegatedAuthor()
	{
		var tokens = Parse("-author:maras");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostAuthorToken("maras", true));
	}

	[Test]
	public async Task TestParseNegatedAuthorCaseInsensitive()
	{
		var tokens = Parse("-author:  Maras  ");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new BlogpostAuthorToken("Maras", true));
	}
}