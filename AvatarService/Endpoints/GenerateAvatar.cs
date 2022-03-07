using AvatarService.Infrastructure;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AvatarService.Endpoints;

public class GenerateAvatar
{
    private const int BaseSize = 200;
    
    public async Task<byte[]> Generate(string name, int width = BaseSize, int height = BaseSize)
    {
        var maxSize = width > height ? width : height;
        var initials = Initials(name);
        
        // Generate colours
        var seed = name.GetDeterministicHashCode();
        var rng = new Random(seed);

        var hue = rng.Next(0, 360);
        var saturation = (float)(rng.NextDouble() * 0.8 + 0.1);
        var lightness = (float)(rng.NextDouble() * 0.8 + 0.1);
        
        var startColor = new Hsl(hue, saturation, lightness + 0.05f).GetRgbColor();
        var endColor = new Hsl(hue, saturation, lightness - 0.05f).GetRgbColor();
        var textColor = lightness <= 0.5 
            ? Color.FromRgba(255, 255, 255, 200) 
            : Color.FromRgba(0, 0, 0, 200);

        
        // Calculate font size
        var size = initials.Length switch
        {
            1 => 120,
            2 => 120,
            3 => 90,
            4 => 75,
            _ => 60
        };

        // Generate image
        using var image = new Image<Rgba32>(width, height, Color.White);
        image.Mutate(i =>
        {
            var colorStops = new[]
            {
                new ColorStop(0f, startColor), 
                new ColorStop(1f, endColor)
            };
            i.Fill(new RadialGradientBrush(new PointF(width * 0.1f, height * 0.1f), maxSize, GradientRepetitionMode.None, colorStops));
        });
        image.Mutate(i =>
        {
            var font = SystemFonts.CreateFont("Arial", size, FontStyle.Regular);
            var options = new TextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Origin = new PointF(width * 0.5f, height * 0.5f)
            };
            i.DrawText(options, initials, textColor);
        });
        // image.Mutate(i => i.Vignette(Rgba32.ParseHex("000000b2")));
        
        var ms = new MemoryStream();

        await image.SaveAsync(ms, new WebpEncoder());
        ms.Seek(0, SeekOrigin.Begin);
        
        return ms.ToArray();
    }
    
    private static string Initials(string name) => string.Join(string.Empty, name
        .Split(' ', '_', '-')
        .Select(s => s.Trim().ToUpper()[0]));
}

public static class GenerateAvatarHelpers
{
    public static WebApplication MapGenerateAvatars(this WebApplication app)
    {
        app
            .MapGet("avatar/{name}", async (string name, int? width, int? height) =>
            {
                var imageStream = await new GenerateAvatar().Generate(name, width ?? 200, height ?? 200);
                return Results.File(imageStream, "image/png");
            })
            .WithName("GenerateAvatar");
        return app;
    }
}