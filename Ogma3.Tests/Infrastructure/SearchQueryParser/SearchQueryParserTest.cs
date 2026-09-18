using Ogma3.Data.Stories;
using Ogma3.Infrastructure.SearchQueryParser;
using Parser = Ogma3.Infrastructure.SearchQueryParser.SearchQueryParser;

namespace Ogma3.Tests.Infrastructure.SearchQueryParser;

public sealed class SearchQueryParserTest
{
	private static IReadOnlyList<SearchToken> Parse(string query) =>
		Parser.Parse(query, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

	private static IReadOnlyList<SearchToken> Parse(string query, Dictionary<string, string> aliases) =>
		Parser.Parse(query, aliases);

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
		await Assert.That(tokens[0]).IsEqualTo(new TagToken(null, "dragons"));
	}

	[Test]
	public async Task TestParseTagTrimsWhitespace()
	{
		var tokens = Parse("  dragons  ");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new TagToken(null, "dragons"));
	}

	[Test]
	public async Task TestParseMultipleTags()
	{
		var tokens = Parse(" dragons , magic ");
		await Assert.That(tokens.Count).IsEqualTo(2);
		await Assert.That(tokens[0]).IsEqualTo(new TagToken(null, "dragons"));
		await Assert.That(tokens[1]).IsEqualTo(new TagToken(null, "magic"));
	}

	[Test]
	public async Task TestParseNegatedTag()
	{
		var tokens = Parse("-dragons");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new TagToken(null, "dragons", true));
	}

	[Test]
	public async Task TestParseTitle()
	{
		var tokens = Parse("\"the dragon riders\"");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new TitleToken("the dragon riders"));
	}

	[Test]
	public async Task TestParseTitleTrimsWhitespace()
	{
		var tokens = Parse("\"  spaced  \"");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new TitleToken("spaced"));
	}

	[Test]
	public async Task TestParseCommaInsideQuotesIsNotASeparator()
	{
		var tokens = Parse("\"a, b\", tag");
		await Assert.That(tokens.Count).IsEqualTo(2);
		await Assert.That(tokens[0]).IsEqualTo(new TitleToken("a, b"));
		await Assert.That(tokens[1]).IsEqualTo(new TagToken(null, "tag"));
	}

	[Test]
	public async Task TestParseAuthor()
	{
		var tokens = Parse("author:maras");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new AuthorToken("maras"));
	}

	[Test]
	public async Task TestParseAuthorTrimsWhitespace()
	{
		var tokens = Parse("author:  maras  ");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new AuthorToken("maras"));
	}

	[Test]
	public async Task TestParseNegatedAuthor()
	{
		var tokens = Parse("-author:maras");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new AuthorToken("maras", true));
	}

	[Test]
	public async Task TestParseStatus()
	{
		var tokens = Parse("status:completed");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new StatusToken("completed"));
	}

	[Test]
	public async Task TestStatusTokenParse()
	{
		await Assert.That(new StatusToken("completed").Status).IsEqualTo(EStoryStatus.Completed);
	}

	[Test]
	public async Task TestStatusTokenParseIsCaseInsensitive()
	{
		await Assert.That(new StatusToken("inprogress").Status).IsEqualTo(EStoryStatus.InProgress);
		await Assert.That(new StatusToken("COMPLETED").Status).IsEqualTo(EStoryStatus.Completed);
		await Assert.That(new StatusToken("OnHiatus").Status).IsEqualTo(EStoryStatus.OnHiatus);
	}

	[Test]
	public async Task TestStatusTokenDisplayNameIsNotParseable()
	{
		await Assert.That(new StatusToken("In Progress").Status).IsNull();
	}

	[Test]
	public async Task TestStatusTokenUnknownIsNull()
	{
		await Assert.That(new StatusToken("bogus").Status).IsNull();
	}

	[Test]
	public async Task TestParseRating()
	{
		var tokens = Parse("rating:everyone");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new RatingToken("everyone"));
	}

	[Test]
	public async Task TestParseNegatedRating()
	{
		var tokens = Parse("-rating:adult");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new RatingToken("adult", true));
	}

	[Test]
	public async Task TestParseMyContent()
	{
		await Assert.That(Parse("my:starred")[0]).IsEqualTo(new MyContentToken(MyContentType.Starred));
		await Assert.That(Parse("my:shelved")[0]).IsEqualTo(new MyContentToken(MyContentType.Shelved));
		await Assert.That(Parse("my:started")[0]).IsEqualTo(new MyContentToken(MyContentType.Started));
	}

	[Test]
	public async Task TestParseMyContentCaseInsensitive()
	{
		var tokens = Parse("my:STARTED");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new MyContentToken(MyContentType.Started));
	}

	[Test]
	public async Task TestParseTagWithNamespace()
	{
		var tokens = Parse("fanworks:ember");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new TagToken("fanworks", "ember"));
	}

	[Test]
	public async Task TestParseTagWithNamespaceAlias()
	{
		var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
			["fanworks"] = "fanwork",
		};
		var tokens = Parse("fanworks:ember", aliases);
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new TagToken("fanwork", "ember"));
	}

	[Test]
	public async Task TestParseTagWithSpacedNamespaceUsesAlias()
	{
		var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
			["fanworks"] = "fanwork",
		};
		var tokens = Parse("fan works:ember", aliases);
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new TagToken("fanwork", "ember"));
	}

	[Test]
	public async Task TestParseTagWithSpacedNamespaceWithoutAlias()
	{
		var tokens = Parse("fan works:ember");
		await Assert.That(tokens.Count).IsEqualTo(1);
		await Assert.That(tokens[0]).IsEqualTo(new TagToken("fanworks", "ember"));
	}

	[Test]
	public async Task TestTagTokenFullName()
	{
		await Assert.That(new TagToken(null, "Value").FullName).IsEqualTo("Value");
	}

	[Test]
	public async Task TestTagTokenFullNameWithNamespaceIsLowercased()
	{
		await Assert.That(new TagToken("NS", "Value").FullName).IsEqualTo("ns:value");
	}
}