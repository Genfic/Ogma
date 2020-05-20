using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using B2Net;
using B2Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Ogma3.Services
{
    public class FileUploader : IFileUploader
    {
        private readonly IB2Client _b2Client;
        private readonly IConfiguration _config;

        public FileUploader(IB2Client b2Client, IConfiguration config)
        {
            _b2Client = b2Client;
            _config = config;
        }

        public async Task<FileUploaderResult> Upload(IFormFile cover, string folder, string name, int tries = 10)
        {
            if (cover == null || cover.Length <= 0) 
                throw new ArgumentException("Cover cannot be null or empty");
            
            var ext = cover.FileName.Split('.').Last();
            var fileName = $"{folder}/{name}.{ext}";
                
            await using var ms = new MemoryStream();
            await cover.CopyToAsync(ms);

            var counter = tries;
            while (counter >= 0)
            {
                try
                {
                    var file = await _b2Client.Files.Upload(ms.ToArray(), fileName);
                    return new FileUploaderResult
                    {
                        FileId = file.FileId,
                        Path = _config["cdn"] + fileName
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
        
        public class FileUploaderResult
        {
            public string FileId { get; set; }
            public string Path { get; set; }
        }
    }
}