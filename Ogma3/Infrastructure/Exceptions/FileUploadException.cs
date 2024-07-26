namespace Ogma3.Infrastructure.Exceptions;

public class FileUploadException : Exception
{
	public FileUploadException(string message, string fileName, long fileSize) : base(message)
	{
		base.Data.Add("file name", fileName);
		base.Data.Add("file size", fileSize);
	}
}