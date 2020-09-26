using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using B2Net;
using B2Net.Models;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Services.FileUploader
{
    public class FileUploader : IFileUploader
    {
        private readonly IB2Client _b2Client;

        public FileUploader(IB2Client b2Client)
        {
            _b2Client = b2Client;
        }

        /// <inheritdoc cref="IFileUploader"/>
        /// <exception cref="ArgumentException">Thrown when the given file is null or empty</exception>
        /// <exception cref="Exception">Thrown when after `tries` amount of tries file could not be uploaded</exception>
        public async Task<FileUploaderResult> Upload(IFormFile file, string folder, string name, int tries = 10)
        {
            if (file == null || file.Length <= 0) 
                throw new ArgumentException("File cannot be null or empty");
            
            var ext = file.FileName.Split('.').Last();
            var fileName = $"{folder}/{name}.{ext}";
                
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var counter = tries;
            while (counter >= 0)
            {
                try
                {
                    var result = await _b2Client.Files.Upload(ms.ToArray(), fileName);
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