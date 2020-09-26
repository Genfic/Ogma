using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Services.FileUploader
{
    public interface IFileUploader
    {
        /// <summary>
        /// Uploads a given file to some persistent storage
        /// </summary>
        /// <param name="file">File to be uploaded</param>
        /// <param name="folder">Folder – or a whole path – to upload the file to on the storage</param>
        /// <param name="name">Name of the file</param>
        /// <param name="tries">How many times should the upload be attempted</param>
        /// <returns>`FileUploaderResult` object</returns>
        Task<FileUploaderResult> Upload(IFormFile file, string folder, string name, int tries = 10);
    }
}