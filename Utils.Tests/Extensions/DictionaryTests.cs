using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class DictionaryTests
{
	[Test]
	public async Task AddMany_EmptyDictionary()
	{
		var dict = new Dictionary<string, int>();
		var other = new Dictionary<string, int>();
		dict.AddMany(other);
		
		await Assert.That(dict).IsEmpty();
	}

	[Test]
	public async Task AddMany_ReplaceTrue()
	{
		var dict = new Dictionary<string, int> { ["a"] = 1 };
		var other = new Dictionary<string, int> { ["a"] = 2, ["b"] = 3 };
		dict.AddMany(other, replace: true);
		
		await Assert.That(dict.Count()).IsEqualTo(2);
		await Assert.That(dict["a"]).IsEqualTo(2);
		await Assert.That(dict["b"]).IsEqualTo(3);
	}

	[Test]
	public async Task AddMany_ReplaceFalse()
	{
		var dict = new Dictionary<string, int> { ["a"] = 1 };
		var other = new Dictionary<string, int> { ["a"] = 2, ["b"] = 3 };
		dict.AddMany(other, replace: false);
		
		await Assert.That(dict.Count()).IsEqualTo(2);
		await Assert.That(dict["a"]).IsEqualTo(1);
		await Assert.That(dict["b"]).IsEqualTo(3);
	}

	[Test]
	public async Task GetOrDefault_ExistingKey()
	{
		var dict = new Dictionary<string, int> { ["a"] = 1 };
		var result = dict.GetOrDefault("a");
		
		await Assert.That(result).IsEqualTo(1);
	}

	[Test]
	public async Task GetOrDefault_NonExistingKey()
	{
		var dict = new Dictionary<string, int> { ["a"] = 1 };
		var result = dict.GetOrDefault("b");
		
		await Assert.That(result).IsEqualTo(0);
	}

	[Test]
	public async Task GetOrDefault_WithDefaultValue()
	{
		var dict = new Dictionary<string, int> { ["a"] = 1 };
		var result = dict.GetOrDefault("b", 42);
		
		await Assert.That(result).IsEqualTo(42);
	}

	[Test]
	public async Task GetOrDefault_StringDictionary()
	{
		var dict = new Dictionary<string, string> { ["key"] = "value" };
		var result = dict.GetOrDefault("nonexistent", "default");
		
		await Assert.That(result).IsEqualTo("default");
	}
}
