using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

namespace AvatarService.Infrastructure;

public static class ColorHelpers
{
    public static Color GetRgbColor(this Hsl hsl)
    {
        var converter = new ColorSpaceConverter();
        return new Color((Rgba32)converter.ToRgb(hsl));
    }
}