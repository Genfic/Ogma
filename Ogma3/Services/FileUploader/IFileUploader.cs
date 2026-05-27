namespace Ogma3.Services.FileUploader;

public interface IFileUploader
{
	/// <inheritdoc cref="Upload(byte[], string, int)"/>
	Task<FileUploadResult> Upload(byte[] bytes, string folder, int tries = 5);

	/// <summary>
	/// Uploads a given file to some persistent storage
	/// </summary>
	/// <inheritdoc cref="Upload(byte[], string, int)"/>
	/// <param name="bytes">File to be uploaded</param>
	/// <param name="folder">Folder – or a whole path – to upload the file to on the storage</param>
	/// <param name="name">Name of the file</param>
	/// <param name="tries">The number of times the upload should be attempted</param>
	/// <returns>`FileUploaderResult` object</returns>
	Task<FileUploadResult> Upload(byte[] bytes, string folder, string name,int tries = 5);

	/// <summary>
	/// Delete the file from the blob storage
	/// </summary>
	/// <param name="key">Name of the file</param>
	/// <param name="cancellationToken">Cancellation token</param>
	Task Delete(string key, CancellationToken cancellationToken = default);
}