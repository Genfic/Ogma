using System;
using System.Collections.Generic;
using Utils.Extensions;
using Xunit;

namespace Utils.Tests.Utils;

public sealed class ExtensionsTest
{
	// Test IComparable.Between()
	[Fact]
	public void TestBetween_Between()
		=> Assert.True(10.Between(1, 100));

	[Theory]
	[InlineData(11, 100)]
	[InlineData(1, 9)]
	public void TestBetween_Outside(int a, int b)
		=> Assert.False(10.Between(a, b));


	// Test double.Normalize()
	[Fact]
	public void TestNormalize_IncorrectOldRange()
		=> Assert.Throws<ArgumentException>(() => 10.0.Normalize(100.0, 10.0));

	[Fact]
	public void TestNormalize_IncorrectNewRange()
		=> Assert.Throws<ArgumentException>(() => 10.0.Normalize(0.0, 100.0, 9.0, 2.0));

	[Fact]
	public void TestNormalize_CorrectRange()
		=> Assert.Equal(0.25, 25.0.Normalize(0.0, 100.0));

	// Test clamp
	[Fact]
	public void TestClamp_InRange()
		=> Assert.Equal(5, 5.Clamp(0, 10));

	[Fact]
	public void TestClamp_OutLeft()
		=> Assert.Equal(5, 1.Clamp(5, 10));

	[Fact]
	public void TestClamp_OutRight()
		=> Assert.Equal(10, 20.Clamp(5, 10));

	[Fact]
	public void TestClamp_IncorrectRange()
		=> Assert.Throws<ArgumentException>(() => 1.Clamp(100, 10));

	// Test string.Friendlify()
	[Fact]
	public void TestFriendlify()
		=> Assert.Equal("abcd-efgh-ijk-l-190-21", "aBcD.eFgH iJk++++++L 190.21".Friendlify());
	
	// Test string.ReplaceWithPattern()
	[Fact]
	public void TestReplaceWithPattern()
	{
		const string inString = "Hello, {{name}}! Goodbye, {{name}}! Have a nice {{time}}.";
		var pattern = new Dictionary<string, string>
		{
			{ "{{name}}", "Bob" },
			{ "{{time}}", "night" },
		};
		const string outString = "Hello, Bob! Goodbye, Bob! Have a nice night.";

		Assert.Equal(outString, inString.ReplaceWithPattern(pattern));
	}

	// Test string.Words()
	[Fact]
	public void TestWords()
	{
		Assert.Equal(3, "one two three".Words());
	}

	[Fact]
	public void TestWords_Empty()
	{
		Assert.Equal(0, "".Words());
	}

	[Fact]
	public void TestWords_MoreWhitespace()
	{
		Assert.Equal(3, "one     two \t three   ".Words());
	}

	[Fact]
	public void TestParseHashtags()
	{
		Assert.Equal(
			new[] { "aaa", "bbb", "ccc" },
			"aaa, bbb, ccc".ParseHashtags()
		);
	}

	[Fact]
	public void TestParseHashtags_WithHashChars()
	{
		Assert.Equal(
			new[] { "aaa", "bbb", "ccc" },
			"#aaa,  #bbb  , #  ccc".ParseHashtags()
		);
	}
	
	// Test find hashtags
	[Fact]
	public void TestFindHashtags_EmptyString()
	{
		Assert.Equal(
			[],
			string.Empty.FindHashtags()
		);
	}
	
	[Fact]
	public void TestFindHashtags_MalformedHashtags()
	{
		Assert.Equal(
			[],
			"#one#two #buckle/my/shoe #three!@#$%^&*()_+-={}[]:\";'<>?,./~`".FindHashtags()
		);
	}
	
	[Fact]
	public void TestFindHashtags_WellFormedHashtags()
	{
		Assert.Equal(
			["#one", "#buckle-my", "#3_4", "#some", "#yeeeeah"],
			"#one two #buckle-my shoe #3_4 buckle #some #m #or #e #yeeeeah".FindHashtags()
		);
	}
}