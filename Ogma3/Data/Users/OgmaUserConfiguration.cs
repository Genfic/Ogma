using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.CommentsThreads;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Users;

public class OgmaUserConfiguration : IEntityTypeConfiguration<OgmaUser>
{
    public void Configure(EntityTypeBuilder<OgmaUser> builder)
    {
        // CONSTRAINTS
        builder
            .Property(u => u.Title)
            .HasMaxLength(CTConfig.CUser.MaxTitleLength);
            
        builder
            .Property(u => u.Bio)
            .HasMaxLength(CTConfig.CUser.MaxBioLength);

        builder
            .Property(u => u.Links)
            .IsRequired()
            .HasMaxLength(CTConfig.CUser.MaxLinksAmount)
            .HasDefaultValueSql(PgConstants.EmptyArray);
            
        builder
            .Property(u => u.RegistrationDate)
            .IsRequired()
            .HasDefaultValueSql(PgConstants.CurrentTimestamp)
            .ValueGeneratedOnAdd();
            
        builder
            .Property(u => u.LastActive)
            .IsRequired()
            .HasDefaultValueSql(PgConstants.CurrentTimestamp)
            .ValueGeneratedOnAdd();
            
        builder
            .Ignore(u => u.PhoneNumber)
            .Ignore(u => u.PhoneNumberConfirmed);
            
        // NAVIGATION
        builder
            .HasOne(u => u.CommentsThread)
            .WithOne(ct => ct.User)
            .HasForeignKey<CommentsThread>(ct => ct.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder
            .HasMany(u => u.Roles)
            .WithMany(or => or.Users)
            .UsingEntity<UserRole>(
                ur => ur.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(k => k.RoleId),
                ur => ur.HasOne(e => e.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(k => k.UserId)
            );
            
        builder
            .HasMany(u => u.Followers)
            .WithMany(u => u.Following)
            .UsingEntity<UserFollow>(
                uf => uf.HasOne(e => e.FollowingUser)
                    .WithMany()
                    .HasForeignKey(k => k.FollowingUserId),
                uf => uf.HasOne(e => e.FollowedUser)
                    .WithMany()
                    .HasForeignKey(k => k.FollowedUserId),
                uf => uf.HasKey(i => new { i.FollowingUserId, i.FollowedUserId })
            );
            
        builder
            .HasMany(u => u.Blockers)
            .WithMany(u => u.Blocking)
            .UsingEntity<UserBlock>(
                ub => ub.HasOne(e => e.BlockingUser)
                    .WithMany()
                    .HasForeignKey(k => k.BlockingUserId),
                ub => ub.HasOne(e => e.BlockedUser)
                    .WithMany()
                    .HasForeignKey(k => k.BlockedUserId),
                ub => ub.HasKey(i => new { i.BlockingUserId, i.BlockedUserId })
            );
            
        builder
            .HasMany(u => u.Reports)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}