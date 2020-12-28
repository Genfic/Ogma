using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Models;
using Ogma3.Infrastructure;

namespace Ogma3.Data.ModelConfigs
{
    public class ModeratorActionConfiguration : BaseConfiguration<ModeratorAction>
    {
        public override void Configure(EntityTypeBuilder<ModeratorAction> builder)
        {
            base.Configure(builder);
            
            // CONSTRAINTS
            builder
                .Property(ma => ma.Description)
                .IsRequired();

            builder
                .Property(ma => ma.DateTime)
                .IsRequired()
                .HasDefaultValueSql(PgConstants.CurrentTimestamp);
            
            // NAVIGATION
            builder
                .HasOne(ma => ma.StaffMember)
                .WithMany()
                .HasForeignKey(ma => ma.StaffMemberId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}