using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Images;

[AutoDbSet]
public sealed class Image : BaseModel
{
	public required string Url { get; set; }
	public string? BackblazeId { get; set; }
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
			.Property(i => i.BackblazeId)
			.HasMaxLength(255);
	}
}