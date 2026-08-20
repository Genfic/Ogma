using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Comments;
using Ogma3.Data.Constants;
using Ogma3.Data.Helpers;

namespace Ogma3.Data.CommentsThreads;

public sealed class CommentsThreadConfiguration : BaseConfiguration<CommentThread>
{
	public override void Configure(EntityTypeBuilder<CommentThread> builder)
	{
		base.Configure(builder);

		// CONSTRAINTS
		builder
			.Property(ct => ct.CommentsCount)
			.IsRequired()
			.HasDefaultValue(0);

		builder
			.Property(ct => ct.IsLocked)
			.HasComputedColumnSql($"\"{nameof(CommentThread.LockDate)}\" IS NOT NULL", true);

		builder
			.Property(ct => ct.LastChange)
			.HasDefaultValueSql(PgConstants.CurrentTimestamp);

		builder.HasPartialIndex(ct => ct.UserId);
		builder.HasPartialIndex(ct => ct.ChapterId);
		builder.HasPartialIndex(ct => ct.BlogpostId);
		builder.HasPartialIndex(ct => ct.ClubThreadId);
		builder.HasPartialIndex(ct => ct.NewsId);

		builder.Property(ct => ct.Source)
			.HasComputedColumnSql(
				$"""
				CASE
					WHEN "{nameof(CommentThread.ChapterId)}" IS NOT NULL THEN {(short)CommentSource.Chapter}
					WHEN "{nameof(CommentThread.BlogpostId)}" IS NOT NULL THEN {(short)CommentSource.Blogpost}
					WHEN "{nameof(CommentThread.UserId)}" IS NOT NULL THEN {(short)CommentSource.Profile}
					WHEN "{nameof(CommentThread.ClubThreadId)}" IS NOT NULL THEN {(short)CommentSource.ForumPost}
					WHEN "{nameof(CommentThread.NewsId)}" IS NOT NULL THEN {(short)CommentSource.NewsPost}
				END
				""", stored: true);

		// NAVIGATION
		builder
			.HasMany(ct => ct.Comments)
			.WithOne(c => c.CommentThread)
			.HasForeignKey(ct => ct.CommentsThreadId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(ct => ct.Subscribers)
			.WithMany(u => u.SubscribedThreads)
			.UsingEntity<CommentThreadSubscriber>(
				cts => cts
					.HasOne(c => c.OgmaUser)
					.WithMany()
					.HasForeignKey(c => c.OgmaUserId),
				cts => cts
					.HasOne(c => c.CommentThread)
					.WithMany()
					.HasForeignKey(c => c.CommentsThreadId)
			);

		// Seed comments for system users
		builder.HasData(
			new
			{
				Id = SystemUserConstants.Deleted.Id,
				UserId = SystemUserConstants.Deleted.Id,
				LockDate = DateTimeOffset.MinValue,
			},
			new
			{
				Id = SystemUserConstants.Anonymous.Id,
				UserId = SystemUserConstants.Anonymous.Id,
				LockDate = DateTimeOffset.MinValue,
			}
		);
	}
}