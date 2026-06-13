using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class ListTests
{
	[Test]
	public async Task AddMany_Empty()
	{
		var list = new List<int>();
		list.AddMany();
		
		await Assert.That(list).IsEmpty();
	}

	[Test]
	public async Task AddMany_SingleItem()
	{
		var list = new List<int>();
		list.AddMany(1);
		
		await Assert.That(list.Count()).IsEqualTo(1);
		await Assert.That(list[0]).IsEqualTo(1);
	}

	[Test]
	public async Task AddMany_MultipleItems()
	{
		var list = new List<int>();
		list.AddMany(1, 2, 3);
		
		await Assert.That(list.Count()).IsEqualTo(3);
		await Assert.That(list[0]).IsEqualTo(1);
		await Assert.That(list[1]).IsEqualTo(2);
		await Assert.That(list[2]).IsEqualTo(3);
	}

	[Test]
	public async Task AddMany_ToExistingList()
	{
		var list = new List<int> { 0 };
		list.AddMany(1, 2, 3);
		
		await Assert.That(list.Count()).IsEqualTo(4);
		await Assert.That(list[0]).IsEqualTo(0);
		await Assert.That(list[1]).IsEqualTo(1);
		await Assert.That(list[2]).IsEqualTo(2);
		await Assert.That(list[3]).IsEqualTo(3);
	}

	[Test]
	public async Task AddMany_WithStrings()
	{
		var list = new List<string>();
		list.AddMany("a", "b", "c");
		
		await Assert.That(list.Count()).IsEqualTo(3);
		await Assert.That(list[0]).IsEqualTo("a");
		await Assert.That(list[1]).IsEqualTo("b");
		await Assert.That(list[2]).IsEqualTo("c");
	}
}
