using AutoDbSetGenerators;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Documents;
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

[AutoDbSet]
public sealed class Document : BaseModel
{
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public DateTimeOffset? RevisionDate { get; set; }
	public DateTimeOffset CreationTime { get; init; }
	public uint Version { get; init; }
	public required string Body { get; init; }
	public string CompiledBody { get; init; } = "";
	public string? CustomCss { get; init; }
	public string? CustomJs { get; init; }
	public List<Header> Headers { get; init; } = [];

	public sealed record Header(byte Level, string Id, string Body);
}