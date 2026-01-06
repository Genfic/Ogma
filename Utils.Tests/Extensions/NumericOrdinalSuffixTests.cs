using System.Numerics;
using Utils.Extensions;

namespace Utils.Tests.Extensions;

public class OrdinalSuffixTests
{
    [Test]
    [InstanceMethodDataSource(nameof(GetSByteTestData))]
    public async Task SByte_Returns_Correct_Suffix(sbyte number, string expectedSuffix)
    {
        var actualSuffix = number.GetOrdinalSuffix();
        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    [Test]
    [InstanceMethodDataSource(nameof(GetByteTestData))]
    public async Task Byte_Returns_Correct_Suffix(byte number, string expectedSuffix)
    {
        var actualSuffix = number.GetOrdinalSuffix();
        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    [Test]
    [InstanceMethodDataSource(nameof(GetShortTestData))]
    public async Task Short_Returns_Correct_Suffix(short number, string expectedSuffix)
    {
        var actualSuffix = number.GetOrdinalSuffix();
        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    [Test]
    [InstanceMethodDataSource(nameof(GetUShortTestData))]
    public async Task UShort_Returns_Correct_Suffix(ushort number, string expectedSuffix)
    {
        var actualSuffix = number.GetOrdinalSuffix();
        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    [Test]
    [InstanceMethodDataSource(nameof(GetIntTestData))]
    public async Task Int_Returns_Correct_Suffix(int number, string expectedSuffix)
    {
        var actualSuffix = number.GetOrdinalSuffix();
        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    [Test]
    [InstanceMethodDataSource(nameof(GetUIntTestData))]
    public async Task UInt_Returns_Correct_Suffix(uint number, string expectedSuffix)
    {
        var actualSuffix = number.GetOrdinalSuffix();
        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    [Test]
    [InstanceMethodDataSource(nameof(GetLongTestData))]
    public async Task Long_Returns_Correct_Suffix(long number, string expectedSuffix)
    {
        var actualSuffix = number.GetOrdinalSuffix();
        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    [Test]
    [InstanceMethodDataSource(nameof(GetULongTestData))]
    public async Task ULong_Returns_Correct_Suffix(ulong number, string expectedSuffix)
    {
        var actualSuffix = number.GetOrdinalSuffix();
        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    [Test]
    [InstanceMethodDataSource(nameof(GetBigIntegerTestData))]
    public async Task BigInteger_Returns_Correct_Suffix(BigInteger number, string expectedSuffix)
    {
        var actualSuffix = number.GetOrdinalSuffix();
        await Assert.That(actualSuffix).IsEqualTo(expectedSuffix);
    }

    // Instance method data sources
    public static IEnumerable<(sbyte, string)> GetSByteTestData() => GenerateTestData<sbyte>();
    public static IEnumerable<(byte, string)> GetByteTestData() => GenerateTestData<byte>();
    public static IEnumerable<(short, string)> GetShortTestData() => GenerateTestData<short>();
    public static IEnumerable<(ushort, string)> GetUShortTestData() => GenerateTestData<ushort>();
    public static IEnumerable<(int, string)> GetIntTestData() => GenerateTestData<int>();
    public static IEnumerable<(uint, string)> GetUIntTestData() => GenerateTestData<uint>();
    public static IEnumerable<(long, string)> GetLongTestData() => GenerateTestData<long>();
    public static IEnumerable<(ulong, string)> GetULongTestData() => GenerateTestData<ulong>();
    public static IEnumerable<(BigInteger, string)> GetBigIntegerTestData() => GenerateTestData<BigInteger>();

    private static IEnumerable<(T, string)> GenerateTestData<T>() where T : IBinaryInteger<T>
    {
        // Standard cases using the generic T
        yield return (T.CreateChecked(0), "th");
        yield return (T.CreateChecked(1), "st");
        yield return (T.CreateChecked(2), "nd");
        yield return (T.CreateChecked(3), "rd");
        yield return (T.CreateChecked(4), "th");
        yield return (T.CreateChecked(10), "th");
        yield return (T.CreateChecked(11), "th");
        yield return (T.CreateChecked(12), "th");
        yield return (T.CreateChecked(13), "th");
        yield return (T.CreateChecked(21), "st");
        yield return (T.CreateChecked(22), "nd");
        yield return (T.CreateChecked(23), "rd");
        yield return (T.CreateChecked(101), "st");
        yield return (T.CreateChecked(111), "th");

        // Handle BigInteger specific large cases
        if (typeof(T) != typeof(BigInteger)) yield break;

        yield return (T.CreateChecked(BigInteger.Parse("0")), "th");
        yield return (T.CreateChecked(BigInteger.Parse("1")), "st");
        yield return (T.CreateChecked(BigInteger.Parse("11")), "th");
        yield return (T.CreateChecked(BigInteger.Parse("22")), "nd");
        yield return (T.CreateChecked(BigInteger.Parse("123456789012345678901234567893")), "rd");
    }
}