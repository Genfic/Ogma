using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Utils.Extensions;

namespace Utils.Tests.Utils;

public sealed class ExtensionsTest
{

	// Test string.Friendlify()
	[Test]
	public async Task TestFriendlify()
	{
		await Assert.That("aBcD.eFgH iJk++++++L 190.21".Friendlify()).IsEqualTo("abcd-efgh-ijk-l-190-21");
	}

	// Test string.ReplaceWithPattern()
	[Test]
	public async Task TestReplaceWithPattern()
	{
		const string inString = "Hello, {{name}}! Goodbye, {{name}}! Have a nice {{time}}.";
		var pattern = new Dictionary<string, string>
		{
			{ "{{name}}", "Bob" },
			{ "{{time}}", "night" },
		};
		const string outString = "Hello, Bob! Goodbye, Bob! Have a nice night.";

		await Assert.That(inString.ReplaceWithPattern(pattern)).IsEqualTo(outString);
	}

	// Test string.Words()
	[Test]
	public async Task TestWords()
	{
		await Assert.That("one two three".Words()).IsEqualTo(3);
	}

	[Test]
	public async Task TestWords_Empty()
	{
		await Assert.That("".Words()).IsEqualTo(0);
	}

	[Test]
	public async Task TestWords_MoreWhitespace()
	{
		await Assert.That("one     two \t three   ".Words()).IsEqualTo(3);
	}

	[Test]
	public async Task TestParseHashtags()
	{
		await Assert
			.That("aaa, bbb, ccc".ParseHashtags())
			.IsEquivalentTo(["aaa", "bbb", "ccc"]);
	}

	[Test]
	public async Task TestParseHashtags_WithHashChars()
	{
		await Assert
			.That("#aaa,  #bbb  , #  ccc".ParseHashtags())
			.IsEquivalentTo(["aaa", "bbb", "ccc"]);
	}

	// Test find hashtags
	[Test]
	public async Task TestFindHashtags_EmptyString()
	{
		await Assert
			.That(string.Empty.FindHashtags())
			.IsEquivalentTo(ImmutableArray<string>.Empty);
	}

	[Test]
	public async Task TestFindHashtags_MalformedHashtags()
	{
		await Assert
			.That("#one#two #buckle/my/shoe #three!@#$%^&*()_+-={}[]:\";'<>?,./~`".FindHashtags())
			.IsEquivalentTo(ImmutableArray<string>.Empty);
	}

	[Test]
	public async Task TestFindHashtags_WellFormedHashtags()
	{
		await Assert
			.That("#one two #buckle-my shoe #3_4 buckle #some #m #or #e #yeeeeah".FindHashtags())
			.IsEquivalentTo(["#one", "#buckle-my", "#3_4", "#some", "#yeeeeah"]);
	}
}