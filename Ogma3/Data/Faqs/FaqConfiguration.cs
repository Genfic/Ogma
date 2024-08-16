using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Faqs;

public class FaqConfiguration : BaseConfiguration<Faq>
{
	public override void Configure(EntityTypeBuilder<Faq> builder)
	{
		base.Configure(builder);

		builder
			.Property(f => f.Question)
			.HasMaxLength(5000)
			.IsRequired();

		builder
			.Property(f => f.Answer)
			.HasMaxLength(10_000)
			.IsRequired();

		builder
			.Property(f => f.AnswerRendered)
			.HasMaxLength(20_000)
			.IsRequired();
	}
}