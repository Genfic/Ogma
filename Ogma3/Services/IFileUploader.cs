using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Services
{
    public interface IFileUploader
    {
        Task<FileUploader.FileUploaderResult> Upload(IFormFile cover, string folder, string name, int tries = 10);
    }
}