using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Models;

namespace Ogma3.Data.ModelConfigs
{
    public class NamespaceConfiguration : BaseConfiguration<Namespace>
    {
        public override void Configure(EntityTypeBuilder<Namespace> builder)
        {
            base.Configure(builder);
            
            // CONSTRAINTS
            builder
                .HasIndex(n => n.Name)
                .IsUnique();

            builder
                .Property(n => n.Name)
                .IsRequired()
                .HasMaxLength(CTConfig.CNamespace.MaxNameLength);

            builder
                .Property(n => n.Color)
                .HasMaxLength(7);

            builder
                .Property(n => n.Order)
                .HasDefaultValue(null);
        }
    }
}