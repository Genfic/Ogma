#nullable disable

using System;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Documents;

public class Document : BaseModel
{
	public string Title { get; init; }
	public string Slug { get; init; }
	public DateTime? RevisionDate { get; set; }
	public DateTime CreationTime { get; init; }
	public uint Version { get; init; }
	public string Body { get; init; }
}