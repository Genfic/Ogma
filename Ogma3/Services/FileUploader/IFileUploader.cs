namespace Ogma3.Services.FileUploader;

public interface IFileUploader
{
	/// <summary>
	/// Uploads a given file to some persistent storage.
	/// File name is automatically set to Guid.NewGuid()
	/// </summary>
	/// <param name="file">File to be uploaded</param>
	/// <param name="folder">Folder – or a whole path – to upload the file to on the storage</param>
	/// <param name="width">Desired width of the uploaded image</param>
	/// <param name="height">Desired height of the uploaded image</param>
	/// <param name="tries">How many times should the upload be attempted</param>
	/// <returns>`FileUploaderResult` object</returns>
	Task<FileUploaderResult> Upload(IFormFile file, string folder, int? width = null, int? height = null, int tries = 10);

	/// <summary>
	/// Uploads a given file to some persistent storage
	/// </summary>
	/// <param name="file">File to be uploaded</param>
	/// <param name="folder">Folder – or a whole path – to upload the file to on the storage</param>
	/// <param name="name">Name of the file</param>
	/// <param name="width">Desired width of the uploaded image</param>
	/// <param name="height">Desired height of the uploaded image</param>
	/// <param name="tries">How many times should the upload be attempted</param>
	/// <returns>`FileUploaderResult` object</returns>
	Task<FileUploaderResult> Upload(IFormFile file, string folder, string name, int? width = null, int? height = null, int tries = 10);

	/// <summary>
	/// Delete the file from the blob storage
	/// </summary>
	/// <param name="name">Name of the file</param>
	/// <param name="id">ID of the file</param>
	/// <param name="cancellationToken">Cancellation token</param>
	Task Delete(string name, string id, CancellationToken cancellationToken = default);
}