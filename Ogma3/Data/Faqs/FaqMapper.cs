using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Faqs;

[Mapper]
public static partial class FaqMapper
{
	public static partial IQueryable<FaqDto> ProjectToDto(this IQueryable<Faq> q);
	public static partial FaqDto ToDto(this Faq faq);
}
