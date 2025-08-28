namespace Ogma3.Services.FileLogService;

public interface IFileLogService
{
	string FileName { get; set; }
	Task Write(string message, CancellationToken cancellationToken);
	IEnumerable<string> ReadLines();
	string ReadAll();
}