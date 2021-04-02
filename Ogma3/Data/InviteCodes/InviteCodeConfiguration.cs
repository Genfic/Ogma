using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Infrastructure;

namespace Ogma3.Data.InviteCodes
{
    public class InviteCodeConfiguration : BaseConfiguration<InviteCode>
    {
        public override void Configure(EntityTypeBuilder<InviteCode> builder)
        {
            base.Configure(builder);
            
            // CONSTRAINTS
            builder
                .Property(ic => ic.Code)
                .IsRequired();

            builder
                .Property(ic => ic.NormalizedCode)
                .IsRequired();

            builder
                .Property(ic => ic.UsedDate)
                .HasDefaultValue(null);

            builder
                .Property(ic => ic.IssueDate)
                .IsRequired()
                .HasDefaultValueSql(PgConstants.CurrentTimestamp);
            
            // NAVIGATION
            builder
                .HasOne(c => c.UsedBy)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasOne(c => c.IssuedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}