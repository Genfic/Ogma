using AvatarService.Generators;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AvatarService.Endpoints;

public class GenerateAvatar
{
    public async Task<byte[]> Generate(string name, int width = 200, int height = 200)
    {
        var maxSize = width > height ? width : height;
        var initials = TextGenerators.Initials(name);
        
        using var image = new Image<Rgba32>(width, height, Rgba32.ParseHex("#ff0000"));
        image.Mutate(i =>
        {
            var colorStops = new[]
            {
                new ColorStop(0f, Rgba32.ParseHex("#ff0000ff")), 
                new ColorStop(1f, Rgba32.ParseHex("#ff9999ff"))
            };
            i.Fill(new RadialGradientBrush(new PointF(width * 0.1f, height * 0.1f), maxSize, GradientRepetitionMode.None, colorStops));
        });
        image.Mutate(i =>
        {
            var font = SystemFonts.CreateFont("Arial", 120, FontStyle.Bold);
            var options = new TextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Origin = new PointF(width * 0.5f, height * 0.5f)
            };
            i.DrawText(options, initials, Color.Black);
        });
        // image.Mutate(i => i.Vignette(Rgba32.ParseHex("000000b2")));
        
        var ms = new MemoryStream();

        await image.SaveAsync(ms, new PngEncoder());
        ms.Seek(0, SeekOrigin.Begin);
        
        return ms.ToArray();
    }
}

public static class GenerateAvatarHelpers
{
    public static WebApplication MapGenerateAvatars(this WebApplication app)
    {
        app
            .MapGet("avatar/{name}", async (string name) =>
            {
                var imageStream = await new GenerateAvatar().Generate(name);
                return Results.File(imageStream, "image/png");
            })
            .WithName("GenerateAvatar");
        return app;
    }
}