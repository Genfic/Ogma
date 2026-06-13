using System.Collections;
using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class EnumerableExtensionsTests
{
	// Test Tap() - Note: In RELEASE mode, Tap just passes through, in DEBUG it logs
	[Test]
	public async Task Tap_PassesThroughElements()
	{
		var input = new[] { 1, 2, 3 };
		var result = input.Tap(x => x.ToString()).ToList();
		
		await Assert.That(result.Count()).IsEqualTo(3);
		await Assert.That(result[0]).IsEqualTo(1);
		await Assert.That(result[1]).IsEqualTo(2);
		await Assert.That(result[2]).IsEqualTo(3);
	}

	[Test]
	public async Task Tap_EmptyCollection()
	{
		var input = Array.Empty<int>();
		var result = input.Tap(x => x.ToString()).ToList();
		
		await Assert.That(result).IsEmpty();
	}

	[Test]
	public async Task Tap_SingleElement()
	{
		var input = new[] { 42 };
		var result = input.Tap(x => x.ToString()).ToList();
		
		await Assert.That(result.Count()).IsEqualTo(1);
		await Assert.That(result[0]).IsEqualTo(42);
	}

	// Test GetValues()
	[Test]
	public async Task GetValues_FiltersAndFlattens()
	{
		var groupings = new IGrouping<int, string>[]
		{
			new MockGrouping(1, ["a", "b"]),
			new MockGrouping(2, ["c", "d"]),
			new MockGrouping(3, ["e", "f"])
		};
		
		var result = groupings.GetValues(key => key == 1 || key == 2).ToList();
		
		await Assert.That(result.Count()).IsEqualTo(4);
		await Assert.That(result).Contains("a");
		await Assert.That(result).Contains("b");
		await Assert.That(result).Contains("c");
		await Assert.That(result).Contains("d");
		await Assert.That(result).DoesNotContain("e");
		await Assert.That(result).DoesNotContain("f");
	}

	[Test]
	public async Task GetValues_Empty()
	{
		var groups = new List<IGrouping<int, string>>();
		var result = groups.GetValues(key => true).ToList();
		
		await Assert.That(result).IsEmpty();
	}

	[Test]
	public async Task GetValues_NoMatchingGroups()
	{
		var groups = new IGrouping<int, string>[]
		{
			new MockGrouping(1, ["a", "b"]),
			new MockGrouping(2, ["c"])
		};
		
		var result = groups.GetValues(key => key > 10).ToList();
		
		await Assert.That(result).IsEmpty();
	}

	[Test]
	public async Task GetValues_AllMatchingGroups()
	{
		var groups = new IGrouping<int, string>[]
		{
			new MockGrouping(1, ["a"]),
			new MockGrouping(2, ["b"])
		};
		
		var result = groups.GetValues(key => key > 0).ToList();
		
		await Assert.That(result.Count()).IsEqualTo(2);
		await Assert.That(result).Contains("a");
		await Assert.That(result).Contains("b");
	}

	// Helper class to mock IGrouping
	private sealed class MockGrouping : IGrouping<int, string>
	{
		public int Key { get; }
		private readonly IEnumerable<string> _items;
		
		public MockGrouping(int key, IEnumerable<string> items)
		{
			Key = key;
			_items = items;
		}
		
		public IEnumerator<string> GetEnumerator() => _items.GetEnumerator();
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
