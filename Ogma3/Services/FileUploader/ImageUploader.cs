using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using B2Net;
using B2Net.Models;
using Microsoft.AspNetCore.Http;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Ogma3.Services.FileUploader
{
    public class ImageUploader : IFileUploader
    {
        private readonly IB2Client _b2Client;

        public ImageUploader(IB2Client b2Client)
        {
            _b2Client = b2Client;
        }

        /// <inheritdoc cref="IFileUploader"/>
        /// <exception cref="ArgumentException">Thrown when the given file is null or empty</exception>
        /// <exception cref="Exception">Thrown when after `tries` amount of tries file could not be uploaded</exception>
        public async Task<FileUploaderResult> Upload(
            IFormFile file, 
            string folder, 
            string name, 
            int? width = null, 
            int? height = null, 
            int tries = 10
        ) {
            if (file is not {Length: > 0}) 
                throw new ArgumentException("File cannot be null or empty");
            
            // Read file extension
            var ext = file.FileName.Split('.').Last();
                
            // Load the image to a memory stream
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            if (width is not null || height is not null)
            {
                Log.Information($">>> Resizing image {name}");
                
                // Create the appropriate decoder
                IImageDecoder decoder = ext.ToUpper() switch
                {
                    "JPG"  => new JpegDecoder(),
                    "JPEG" => new JpegDecoder(),
                    "PNG"  => new PngDecoder(),
                    "GIF"  => new GifDecoder(),
                    _      => throw new ArgumentException("Unknown file format")
                };
                
                // Reset memory stream position
                ms.Seek(0, SeekOrigin.Begin);
                
                // Load and resize the image
                using var img = await Image.LoadAsync(ms, decoder);
                img.Mutate(i => i.Resize(new ResizeOptions
                {
                    Size = new Size((int)(width ?? height)!, (int)(height ?? width)!),
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center
                }));
                
                // Save it as PNG
                ms.Seek(0, SeekOrigin.Begin);
                await img.SaveAsync(ms, new PngEncoder());
                ext = "png";
            }

            // Assemble the final path
            var fileName = $"{folder}/{name}.{ext}";
            
            // Try to upload the image to the bucket
            var counter = tries;
            while (counter >= 0)
            {
                try
                {
                    var result = await _b2Client.Files.Upload(ms.ToArray(), fileName);
                    Log.Information($">>> Uploaded image {name} to {fileName}");
                    return new FileUploaderResult
                    {
                        FileId = result.FileId,
                        Path = fileName
                    };
                }
                catch (B2Exception e)
                {
                    Console.WriteLine($"⚠ Backblaze Error: {e.Message}");
                    Console.WriteLine($"  Tries left: {--counter}");
                }
            }
            throw new Exception("Could not upload file. Check server logs.");

        }
    }
}