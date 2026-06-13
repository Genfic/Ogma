using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class CollectionsTests
{

	[Test]
	public async Task JoinToString_IEnumerable_CharSeparator()
	{
		IEnumerable<string> items = ["a", "b", "c"];
		var result = items.JoinToString(',');

		await Assert.That(result).IsEqualTo("a,b,c");
	}

	[Test]
	public async Task JoinToString_StringArray_CharSeparator()
	{
		string?[] items = ["a", "b", "c"];
		var result = items.JoinToString(',');

		await Assert.That(result).IsEqualTo("a,b,c");
	}

	//[Test]
	//public async Task JoinToString_IEnumerable_StringSeparator()
	//{
	//	IEnumerable<string> items = new[] { "a", "b", "c" };
	//	var result = global::Utils.Extensions.Enumerable.JoinToString(items, ", ");
	//
	//	await Assert.That(result).IsEqualTo("a, b, c");
	//}

	[Test]
	public async Task JoinToString_StringArray_StringSeparator()
	{
		string?[] items = ["a", "b", "c"];
		var result = items.JoinToString(", ");

		await Assert.That(result).IsEqualTo("a, b, c");
	}

	[Test]
	public async Task JoinToString_EmptyArray()
	{
		string?[] items = [];
		var result = items.JoinToString(',');

		await Assert.That(result).IsEqualTo("");
	}

	[Test]
	public async Task JoinToString_SingleItem()
	{
		string?[] items = ["single"];
		var result = items.JoinToString('|');

		await Assert.That(result).IsEqualTo("single");
	}

	[Test]
	public async Task JoinToString_NullItems()
	{
		string?[] items = [null, "a", null, "b"];
		var result = items.JoinToString('-');

		await Assert.That(result).IsEqualTo("-a--b");
	}

	[Test]
	public async Task JoinToString_DefaultStringSeparator()
	{
		IEnumerable<string> items = ["a", "b"];
		var result = items.JoinToString("");

		await Assert.That(result).IsEqualTo("ab");
	}

	[Test]
	public async Task JoinToString_DefaultCharSeparator()
	{
		string?[] items = ["a", "b"];
		// Call the char overload explicitly
		var result = items.JoinToString(' ');

		await Assert.That(result).IsEqualTo("a b");
	}

	// Test Keyed()
	[Test]
	public async Task Keyed_Empty()
	{
		var input = Array.Empty<string>();
		var result = input.Keyed().ToList();

		await Assert.That(result).IsEmpty();
	}

	[Test]
	public async Task Keyed_SingleItem()
	{
		var input = new[] { "a" };
		var result = input.Keyed().ToList();

		await Assert.That(result.Count).IsEqualTo(1);
		await Assert.That(result[0].key).IsEqualTo(0);
		await Assert.That(result[0].value).IsEqualTo("a");
	}

	[Test]
	public async Task Keyed_MultipleItems()
	{
		var input = new[] { "a", "b", "c" };
		var result = input.Keyed().ToList();

		await Assert.That(result.Count).IsEqualTo(3);
		await Assert.That(result[0]).IsEqualTo((0, "a"));
		await Assert.That(result[1]).IsEqualTo((1, "b"));
		await Assert.That(result[2]).IsEqualTo((2, "c"));
	}

	[Test]
	public async Task Keyed_WithNumbers()
	{
		var input = new[] { 10, 20, 30 };
		var result = input.Keyed().ToList();

		await Assert.That(result.Count).IsEqualTo(3);
		await Assert.That(result[0]).IsEqualTo((0, 10));
		await Assert.That(result[1]).IsEqualTo((1, 20));
		await Assert.That(result[2]).IsEqualTo((2, 30));
	}
}
