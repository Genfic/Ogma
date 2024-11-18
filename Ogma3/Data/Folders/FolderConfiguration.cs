using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Folders;

public sealed class FolderConfiguration : BaseConfiguration<Folder>
{
	public override void Configure(EntityTypeBuilder<Folder> builder)
	{
		base.Configure(builder);

		// CONSTRAINTS
		builder
			.Property(f => f.Name)
			.IsRequired()
			.HasMaxLength(CTConfig.CFolder.MaxNameLength);

		builder
			.Property(f => f.Slug)
			.IsRequired()
			.HasMaxLength(CTConfig.CFolder.MaxNameLength);

		builder
			.Property(f => f.Description)
			.HasMaxLength(CTConfig.CFolder.MaxDescriptionLength);

		builder
			.Property(f => f.AccessLevel)
			.IsRequired()
			.HasDefaultValue(EClubMemberRoles.User);
			// TODO: (dotnet/efcore/#35142) .HasSentinel((EClubMemberRoles)MinusOne());

		// NAVIGATION

		builder
			.HasMany(f => f.Stories)
			.WithMany(s => s.Folders)
			.UsingEntity<FolderStory>(
				fs => fs.HasOne(f => f.Story)
					.WithMany()
					.HasForeignKey(f => f.StoryId)
					.IsRequired(),
				fs => fs.HasOne(f => f.Folder)
					.WithMany()
					.HasForeignKey(f => f.FolderId)
					.IsRequired(),
				fs =>
				{
					fs.HasOne(f => f.AddedBy)
						.WithMany()
						.HasForeignKey(f => f.AddedById);
					fs.Property(f => f.Added)
						.HasDefaultValueSql(PgConstants.CurrentTimestamp);
				}
			);
	}
}