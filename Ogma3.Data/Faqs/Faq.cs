using AutoDbSetGenerators;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Faqs;

[AutoDbSet]
public sealed class Faq : BaseModel
{
	public required string Question { get; set; }
	public required string Answer { get; set; }
	public required string AnswerRendered { get; set; }
}
