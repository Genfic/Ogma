using System;
using Utils;
using Xunit;

namespace Tests.Utils
{
    public class ExtensionsTest
    {
        // Test IComparable.Between()
        [Fact]
        public void TestBetween_Between() 
            => Assert.True(10.Between(1, 100));
        
        [Fact]
        public void TestBetween_OutLeft()
            => Assert.False(1.Between(10, 100));
        
        [Fact]
        public void TestBetween_OutRight() 
            => Assert.False(100.Between(1, 10));
        
        // Test double.Normalize()
        [Fact]
        public void TestNormalize_IncorrectOldRange() 
            => Assert.Throws<ArgumentException>(() => 10.0.Normalize(100.0, 10.0));
        
        [Fact]
        public void TestNormalize_IncorrectNewRange()
            => Assert.Throws<ArgumentException>(() => 10.0.Normalize(0.0, 100.0, 9.0, 2.0));

        [Fact]
        public void TestNormalize_CorrectRange()
            => Assert.Equal(0.25, 25.0.Normalize(0.0, 100.0));
        
        // Test clamp
        [Fact]
        public void TestClamp_InRange()
            => Assert.Equal(5, 5.Clamp(0, 10));

        [Fact]
        public void TestClamp_OutLeft()
            => Assert.Equal(5, 1.Clamp(5, 10));

        [Fact]
        public void TestClamp_OutRight()
            => Assert.Equal(10, 20.Clamp(5, 10));

        [Fact]
        public void TestClamp_IncorrectRange()
            => Assert.Throws<ArgumentException>(() => 1.Clamp(100, 10));
        
        // Test string.Friendlify()
        [Fact]
        public void TestFriendlify()
            => Assert.Equal("abcd_efgh_ijk_l_190_21", "aBcD.eFgH iJk++++++L 190.21".Friendlify());
        
        // Test Color.ToHexCss()
        [Fact]
        public void TestToHexCss()
            => Assert.Equal("#FF0000", System.Drawing.Color.FromArgb(255, 0, 0).ToHexCss());
        
        // Test Color.ToCommaSeparatedCss()
        [Fact]
        public void TestToCommaSeparatedCss()
            => Assert.Equal("255, 0, 0, 1.00", System.Drawing.Color.FromArgb(255,255, 0, 0).ToCommaSeparatedCss());
    }
}