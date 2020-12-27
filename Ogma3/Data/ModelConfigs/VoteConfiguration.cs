using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Models;

namespace Ogma3.Data.ModelConfigs
{
    public class VoteConfiguration : BaseConfiguration<Vote>
    {
        public override void Configure(EntityTypeBuilder<Vote> builder)
        {
            base.Configure(builder);
            
            builder
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasIndex(v => new { v.UserId, v.StoryId })
                .IsUnique();
        }
    }
}