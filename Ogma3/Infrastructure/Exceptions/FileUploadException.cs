namespace Ogma3.Infrastructure.Exceptions;

public sealed class FileUploadException : Exception
{
	public FileUploadException(string message, string fileName, long fileSize) : base(message)
	{
		Data.Add("file name", fileName);
		Data.Add("file size", fileSize);
	}
}