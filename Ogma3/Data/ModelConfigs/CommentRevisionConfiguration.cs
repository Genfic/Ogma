using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Models;
using Ogma3.Infrastructure;

namespace Ogma3.Data.ModelConfigs
{
    public class CommentRevisionConfiguration : BaseConfiguration<CommentRevision>
    {
        public override void Configure(EntityTypeBuilder<CommentRevision> builder)
        {
            base.Configure(builder);
            
            // CONSTRAINTS
            builder
                .Property(cr => cr.EditTime)
                .IsRequired()
                .HasDefaultValueSql(PgConstants.CurrentTimestamp);

            builder
                .Property(cr => cr.Body)
                .IsRequired()
                .HasMaxLength(CTConfig.CComment.MaxBodyLength);
            
            // NAVIGATION
            builder
                .HasOne(cr => cr.Parent)
                .WithMany(c => c.Revisions)
                .HasForeignKey(cr => cr.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}