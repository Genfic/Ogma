using System;

namespace Utils.Extensions;

public static class Numeric
{
    /// <summary>
    /// Check whether `num` is greater than `min` and less than `max`.
    /// </summary>
    /// <param name="num">Number to check</param>
    /// <param name="min">Minimum</param>
    /// <param name="max">Maximum</param>
    /// <typeparam name="T">Type implementing IComparable</typeparam>
    /// <returns>True if `num` is between `min` and `max`, false otherwise.</returns>
    public static bool Between<T>(this T num, T min, T max) where T : IComparable<T>, IComparable
    {
        return num.CompareTo(min) >= 0 && num.CompareTo(max) <= 0;
    }
        
    /// <summary>
    /// Remaps the given `num` in range `oldMin` to `oldMax` to new range of `newMin` to `newMax`.
    /// </summary>
    /// <param name="num">Number to remap</param>
    /// <param name="oldMin">Original lowest value `num` can take</param>
    /// <param name="oldMax">Original highest value `num` can take</param>
    /// <param name="newMin">New lowest value `num` can take</param>
    /// <param name="newMax">New highest value `num` can take</param>
    /// <returns>Remapped `num`</returns>
    /// <exception cref="ArgumentException">`oldMin` has to be less than `oldMax`, and `newMin` has to be less than `newMax`.</exception>
    public static double Normalize(this double num, double oldMin, double oldMax, double newMin, double newMax)
    {
        if (oldMin >= oldMax || newMin >= newMax)
        {
            throw new ArgumentException("oldMin has to be less than oldMax, and newMin has to be less than newMax");
        }

        return newMin + (num - oldMin) * (newMax - newMin) / (oldMax - oldMin);
    }
        
    /// <summary>
    /// Remaps the given `num` in range `oldMin` to `oldMax` to range of 0..1.
    /// </summary>
    /// <param name="num">Number to remap</param>
    /// <param name="oldMin">Original lowest value `num` can take</param>
    /// <param name="oldMax">Original highest value `num` can take</param>
    /// <returns>Remapped `num`</returns>
    /// <exception cref="ArgumentException">`oldMin` has to be less than `oldMax`, and `newMin` has to be less than `newMax`.</exception>
    public static double Normalize(this double num, double oldMin, double oldMax)
    {
        if (oldMin >= oldMax)
        {
            throw new ArgumentException("oldMin has to be less than oldMax");
        }

        return (num - oldMin) / (oldMax - oldMin);
    }
        
    /// <summary>
    /// Clamps the given IComparable between min and max
    /// </summary>
    /// <param name="num">Number to clamp</param>
    /// <param name="min">Minimum</param>
    /// <param name="max">Maximum</param>
    /// <typeparam name="T">Type implementing IComparable</typeparam>
    /// <returns>Value between min and max</returns>
    /// <exception cref="ArgumentException">`min` has to be less than `max`.</exception>
    public static T Clamp<T>(this T num, T min, T max) where T : IComparable<T>, IComparable
    {
        if (min.CompareTo(max) > 0) throw new ArgumentException("min has to be less than max");
        if (num.CompareTo(min) < 0) return min;
        if (num.CompareTo(max) > 0) return max;
        return num;
    }
}