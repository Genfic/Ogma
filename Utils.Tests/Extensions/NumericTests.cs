using System.Numerics;
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
    [MethodDataSource(nameof(AllTestData))]
    public async Task GetOrdinalSuffix_Returns_Correct_Suffix_For_All_Integer_Types<T>(T number, string expectedSuffix)
        where T : IBinaryInteger<T>
    {
        var actualSuffix = number.GetOrdinalSuffix();

        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    public static IEnumerable<object[]> AllTestData()
    {
        foreach (var data in GetIntegerCases<int>()) yield return data;
        foreach (var data in GetIntegerCases<long>()) yield return data;
        foreach (var data in GetIntegerCases<short>()) yield return data;
        foreach (var data in GetIntegerCases<byte>()) yield return data;
        foreach (var data in GetIntegerCases<uint>()) yield return data;
        foreach (var data in GetIntegerCases<ulong>()) yield return data;
        foreach (var data in GetIntegerCases<ushort>()) yield return data;
        foreach (var data in GetIntegerCases<sbyte>()) yield return data;
        foreach (var data in GetBigIntegerCases()) yield return data;
    }

    private static IEnumerable<object[]> GetIntegerCases<T>() where T : IBinaryInteger<T>
    {
        // Standard cases
        yield return [T.CreateChecked(0), "th"];
        yield return [T.CreateChecked(1), "st"];
        yield return [T.CreateChecked(2), "nd"];
        yield return [T.CreateChecked(3), "rd"];
        yield return [T.CreateChecked(4), "th"];
        yield return [T.CreateChecked(10), "th"];
        yield return [T.CreateChecked(21), "st"];
        yield return [T.CreateChecked(22), "nd"];
        yield return [T.CreateChecked(23), "rd"];
        yield return [T.CreateChecked(101), "st"];

        // Edge cases
        yield return [T.CreateChecked(11), "th"];
        yield return [T.CreateChecked(12), "th"];
        yield return [T.CreateChecked(13), "th"];
        yield return [T.CreateChecked(111), "th"];
    }

    // BigInteger needs a separate method because it's created by parsing a string,
    // not directly from a number like the other types.
    private static IEnumerable<object[]> GetBigIntegerCases()
    {
        yield return [BigInteger.Parse("0"), "th"];
        yield return [BigInteger.Parse("1"), "st"];
        yield return [BigInteger.Parse("11"), "th"];
        yield return [BigInteger.Parse("22"), "nd"];
        yield return [BigInteger.Parse("123456789012345678901234567893"), "rd"];
    }
}