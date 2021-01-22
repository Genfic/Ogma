using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Models;
using Ogma3.Infrastructure;

namespace Ogma3.Data.ModelConfigs
{
    public class NotificationConfiguration : BaseConfiguration<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);

            // CONSTRAINTS
            builder
                .Property(n => n.Body)
                .IsRequired();
            
            builder
                .Property(n => n.DateTime)
                .IsRequired()
                .HasDefaultValueSql(PgConstants.CurrentTimestamp);

            builder
                .Property(n => n.Event)
                .IsRequired();
            
            // NAVIGATION
            builder
                .HasMany(n => n.Recipients)
                .WithMany(u => u.Notifications)
                .UsingEntity<NotificationRecipients>(
                    ent => ent.HasOne(nr => nr.Recipient)
                        .WithMany()
                        .HasForeignKey(nr => nr.RecipientId),
                    ent => ent.HasOne(nr => nr.Notification)
                        .WithMany()
                        .HasForeignKey(nr => nr.NotificationId)
                );

        }
    }
}