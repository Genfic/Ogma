using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Tags
{
    public class TagConfiguration : BaseConfiguration<Tag>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            base.Configure(builder);
            
            // CONSTRAINTS
            builder
                .HasIndex(t => t.Name)
                .IsUnique();

            builder
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(CTConfig.CTag.MaxNameLength);

            builder
                .Property(t => t.Slug)
                .IsRequired()
                .HasMaxLength(CTConfig.CTag.MaxNameLength);

            builder
                .Property(t => t.Description)
                .HasMaxLength(CTConfig.CTag.MaxDescLength);

            builder
                .Property(t => t.Namespace)
                .IsRequired(false)
                .HasDefaultValue(null);

            // NAVIGATION


        }
    }
}