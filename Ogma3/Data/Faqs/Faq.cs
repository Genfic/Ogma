using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Faqs;

[AutoDbSet]
public sealed class Faq : BaseModel
{
	public required string Question { get; set; }
	public required string Answer { get; set; }
	public required string AnswerRendered { get; set; }
}

[Mapper]
public static partial class FaqMapper
{
	public static partial IQueryable<FaqDto> ProjectToDto(this IQueryable<Faq> q);
	public static partial FaqDto ToDto(this Faq faq);
}