using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Models;

namespace Ogma3.Data.ModelConfigs
{
    public class RatingConfiguration : BaseConfiguration<Rating>
    {
        public override void Configure(EntityTypeBuilder<Rating> builder)
        {
            base.Configure(builder);
            
            // CONSTRAINTS
            builder
                .HasIndex(r => r.Name)
                .IsUnique();

            builder
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(CTConfig.CRating.MaxNameLength);

            builder
                .Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(CTConfig.CRating.MaxDescriptionLength);

            builder
                .Property(r => r.Order)
                .IsRequired()
                .HasDefaultValue(0);

            builder
                .Property(r => r.BlacklistedByDefault)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}