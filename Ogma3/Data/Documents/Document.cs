using AutoDbSetGenerators;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Documents;

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
	public List<Header> Headers { get; init; } = [];

	public sealed record Header(byte Level, byte Occurrence, string Body);
}