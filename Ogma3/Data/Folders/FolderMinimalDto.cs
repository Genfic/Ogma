namespace Ogma3.Data.Folders;

public sealed class FolderMinimalDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Slug { get; init; }
}