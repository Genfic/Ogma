using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class SpanTests
{
	[Test]
	public async Task IndexOfBefore_Found()
	{
		var result = "a b c".AsSpan().IndexOfBefore(' ', 2);
		await Assert.That(result).IsEqualTo(1);
	}

	[Test]
	public async Task IndexOfBefore_NotFound()
	{
		var result = "a b c".AsSpan().IndexOfBefore('x', 4);
		await Assert.That(result).IsEqualTo(-1);
	}

	[Test]
	public async Task IndexOfBefore_AtGivenIndex()
	{
		var result = "a b c".AsSpan().IndexOfBefore(' ', 3);
		await Assert.That(result).IsEqualTo(3);
	}

	[Test]
	public async Task IndexOfAfter_Found()
	{
		var first = "a b c".AsSpan().IndexOfAfter(' ', 0);
		await Assert.That(first).IsEqualTo(1);

		var second = "a b c".AsSpan().IndexOfAfter('c', 2);
		await Assert.That(second).IsEqualTo(4);
	}

	[Test]
	public async Task IndexOfAfter_NotFound()
	{
		var result = "a b c".AsSpan().IndexOfAfter('x', 0);
		await Assert.That(result).IsEqualTo(-1);
	}
}