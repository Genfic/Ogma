using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Quotes;

public class Quote : BaseModel
{
    public string Body { get; init; }
    public string Author { get; init; }
        
    public class QuoteConfiguration : BaseConfiguration<Quote>
    {
        public override void Configure(EntityTypeBuilder<Quote> builder)
        {
            base.Configure(builder);
            builder.Property(q => q.Body).IsRequired();
            builder.Property(q => q.Author).IsRequired();
        }
    }
}