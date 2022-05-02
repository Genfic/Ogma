using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Shelves;

public class ShelfConfiguration : BaseConfiguration<Shelf>
{
	public override void Configure(EntityTypeBuilder<Shelf> builder)
	{
		base.Configure(builder);

		// CONSTRAINTS
		builder
			.Property(s => s.Name)
			.IsRequired()
			.HasMaxLength(CTConfig.CShelf.MaxNameLength);

		builder
			.Property(s => s.Description)
			.HasDefaultValue(string.Empty)
			.HasMaxLength(CTConfig.CShelf.MaxDescriptionLength);

		builder
			.Property(s => s.IsDefault)
			.HasDefaultValue(false);

		builder
			.Property(s => s.IsPublic)
			.HasDefaultValue(false);

		builder
			.Property(s => s.IsQuickAdd)
			.HasDefaultValue(false);

		builder
			.Property(s => s.TrackUpdates)
			.HasDefaultValue(false);

		builder
			.Property(s => s.Color)
			.HasMaxLength(7)
			.HasDefaultValue(null);

		// NAVIGATION
		builder
			.HasOne(s => s.Icon)
			.WithMany()
			.OnDelete(DeleteBehavior.SetNull);

		builder
			.HasMany(s => s.Stories)
			.WithMany(s => s.Shelves)
			.UsingEntity<ShelfStory>(
				ent => ent.HasOne(ss => ss.Story)
					.WithMany()
					.HasForeignKey(ss => ss.StoryId)
					.OnDelete(DeleteBehavior.Cascade),
				ent => ent.HasOne(ss => ss.Shelf)
					.WithMany()
					.HasForeignKey(ss => ss.ShelfId)
					.OnDelete(DeleteBehavior.Cascade)
			);
	}
}