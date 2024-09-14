using System;
using System.Drawing;
using Utils.Extensions;
using Xunit;

namespace Utils.Tests.Extensions;

public sealed class ColourTests
{
	// Test Color.ToHexCss()
	[Fact]
	public void TestToHexCss()
		=> Assert.Equal("#FF0000", Color.FromArgb(255, 0, 0).ToHexCss());

	// Test Color.ToCommaSeparatedCss()
	[Fact]
	public void TestToCommaSeparatedCss()
		=> Assert.Equal("255, 0, 0, 1.00", Color.FromArgb(255, 255, 0, 0).ToCommaSeparatedCss());
	
	
	[Theory]
	[InlineData("#12")]
	[InlineData("#12345")]
	[InlineData("#1234567")]
	[InlineData("#123456789")]
	[InlineData("#ZZZZZZ")]
	public void TestParseHexColor_Throws(string color)
		=> Assert.Throws<ArgumentException>(() => color.ParseHexColor());
	
	// Test Color.ParseHexColor()
	[Theory]
	[MemberData(nameof(TestParseHexColorData))]
	public void TestParseHexColor(string given, Color expected)
		=> Assert.Equal(expected, given.ParseHexColor());

	public static TheoryData<string, Color> TestParseHexColorData() => new()
	{
		{ "#F00",  Color.FromArgb(0xFF, 0x00, 0x00)},
		{ "#4F00",  Color.FromArgb(0x44,0xFF, 0x00, 0x00)},
		{ "#0F780A",  Color.FromArgb(0x0F, 0x78, 0x0A)},
		{ "#99887766",  Color.FromArgb(0x99, 0x88, 0x77, 0x66)},
		{ "F00",  Color.FromArgb(0xFF, 0x00, 0x00)},
		{ "4F00",  Color.FromArgb(0x44,0xFF, 0x00, 0x00)},
		{ "0F780A",  Color.FromArgb(0x0F, 0x78, 0x0A)},
		{ "99887766",  Color.FromArgb(0x99, 0x88, 0x77, 0x66)},
	};
}