using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using TUnit.Assertions.AssertConditions.Throws;
using Utils.Extensions;

namespace Utils.Tests.Extensions;

public sealed class ColourTests
{
	// Test Color.ToHexCss()
	[Test]
	public async Task TestToHexCss()
	{
		await Assert.That(Color.FromArgb(255, 0, 0).ToHexCss()).IsEqualTo("#FF0000");
	}

	// Test Color.ToCommaSeparatedCss()
	[Test]
	public async Task TestToCommaSeparatedCss()
	{
		await Assert.That(Color.FromArgb(255, 0, 0).ToCommaSeparatedCss()).IsEqualTo("255, 0, 0, 1.00");
	}


	[Test]
	[Arguments("#12")]
	[Arguments("#12345")]
	[Arguments("#1234567")]
	[Arguments("#123456789")]
	[Arguments("#ZZZZZZ")]
	public async Task TestParseHexColor_Throws(string color)
	{
		await Assert.That(color.ParseHexColor).Throws<ArgumentException>();
	}

	// Test Color.ParseHexColor()
	[Test]
	[MethodDataSource(typeof(ColourTestDataSource), nameof(ColourTestDataSource.TestParseHexColorData))]
	public async Task TestParseHexColor((string given, Color expected) data)
	{
		var (given, expected) = data;
		
		await Assert.That(given.ParseHexColor()).IsEqualTo(expected);
	}


}

public static class ColourTestDataSource
{
	public static IEnumerable<Func<(string given, Color expected)>> TestParseHexColorData()
	{
		yield return () => ("#F00", Color.FromArgb(0xFF, 0x00, 0x00));
		yield return () => ("#4F00", Color.FromArgb(0x44, 0xFF, 0x00, 0x00));
		yield return () => ("#0F780A", Color.FromArgb(0x0F, 0x78, 0x0A));
		yield return () => ("#99887766", Color.FromArgb(0x99, 0x88, 0x77, 0x66));
		yield return () => ("F00", Color.FromArgb(0xFF, 0x00, 0x00));
		yield return () => ("4F00", Color.FromArgb(0x44, 0xFF, 0x00, 0x00));
		yield return () => ("0F780A", Color.FromArgb(0x0F, 0x78, 0x0A));
		yield return () => ("99887766", Color.FromArgb(0x99, 0x88, 0x77, 0x66));
	}
}