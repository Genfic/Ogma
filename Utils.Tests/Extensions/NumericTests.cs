using System;
using System.Numerics;
using System.Threading.Tasks;
using TUnit.Assertions.AssertConditions.Throws;
using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class NumericExtensionsTests
{
	// Test IComparable.Between()
	[Test]
	public async Task TestBetween_Between()
	{
		await Assert.That(10.Between(1, 100)).IsTrue();
	}

	[Test]
	[Arguments(11, 100)]
	[Arguments(1, 9)]
	public async Task TestBetween_Outside(int a, int b)
	{
		await Assert.That(10.Between(a, b)).IsFalse();
	}

	// Test double.Normalize()
	[Test]
	public async Task TestNormalize_IncorrectOldRange()
	{
		await Assert.That(() => 10.0.Normalize(100.0, 10.0)).Throws<ArgumentException>();
	}

	[Test]
	public async Task TestNormalize_IncorrectNewRange()
	{
		await Assert.That(() => 10.0.Normalize(0.0, 100.0, 9.0, 2.0)).Throws<ArgumentException>();
	}

	[Test]
	public async Task TestNormalize_CorrectRange()
	{
		await Assert.That(25.0.Normalize(0.0, 100.0)).IsEqualTo(0.25);
	}

	// Test clamp
	[Test]
	public async Task TestClamp_InRange()
	{
		await Assert.That(5.Clamp(0, 10)).IsEqualTo(5);
	}

	[Test]
	public async Task TestClamp_OutLeft()
	{
		await Assert.That(1.Clamp(5, 10)).IsEqualTo(5);
	}

	[Test]
	public async Task TestClamp_OutRight()
	{
		await Assert.That(20.Clamp(5, 10)).IsEqualTo(10);
	}

	[Test]
	public async Task TestClamp_IncorrectRange()
	{
		await Assert.That(() => 1.Clamp(100, 10)).Throws<ArgumentException>();
	}

	// Test GetOrdinalSuffix()
	[Test]
	[Arguments(1)]
	[Arguments(11)]
	[Arguments(21)]
	public async Task TestGetOrdinalSuffix_First<T>(T number) where T : IBinaryInteger<T>
	{
		await Assert.That(number.GetOrdinalSuffix()).IsEqualTo("st");
	}

	[Test]
	[Arguments(2)]
	[Arguments(12)]
	[Arguments(22)]
	public async Task TestGetOrdinalSuffix_Second<T>(T number) where T : IBinaryInteger<T>
	{
		await Assert.That(number.GetOrdinalSuffix()).IsEqualTo("nd");
	}

	[Test]
	[Arguments(3)]
	[Arguments(13)]
	[Arguments(23)]
	public async Task TestGetOrdinalSuffix_Third<T>(T number) where T : IBinaryInteger<T>
	{
		await Assert.That(number.GetOrdinalSuffix()).IsEqualTo("rd");
	}

	[Test]
	[Arguments(4)]
	[Arguments(10)]
	[Arguments(14)]
	[Arguments(20)]
	[Arguments(24)]
	[Arguments(30)]
	public async Task TestGetOrdinalSuffix_Other<T>(T number) where T : IBinaryInteger<T>
	{
		await Assert.That(number.GetOrdinalSuffix()).IsEqualTo("th");
	}
}