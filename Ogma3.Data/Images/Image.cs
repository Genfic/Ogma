using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Constants;

namespace Ogma3.Data.Images;

[AutoDbSet]
public sealed class Image : BaseModel
{
	public required string Url { get; init; }
	public string? ETag { get; init; }
}

public sealed class ImageConfiguration : BaseConfiguration<Image>
{
	public override void Configure(EntityTypeBuilder<Image> builder)
	{
		base.Configure(builder);

		builder
			.Property(i => i.Url)
			.HasMaxLength(255);
		builder
			.Property(i => i.ETag)
			.HasMaxLength(255);

		// Seed avatars for system users
		builder.HasData(
			new()
			{
				Id = SystemUserConstants.Deleted.Id,
				Url = SystemUserConstants.Deleted.Avatar,
			},
			new()
			{
				Id = SystemUserConstants.Anonymous.Id,
				Url = SystemUserConstants.Anonymous.Avatar,
			}
		);
	}
}