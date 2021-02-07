using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Models;

namespace Ogma3.Data.ModelConfigs
{
    public class FaqConfiguration : BaseConfiguration<Faq>
    {
        public override void Configure(EntityTypeBuilder<Faq> builder)
        {
            base.Configure(builder);

            builder
                .Property(f => f.Question)
                .IsRequired();

            builder
                .Property(f => f.Answer)
                .IsRequired();

            builder
                .Property(f => f.AnswerRendered)
                .IsRequired();
        }
    }
}