#nullable disable

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Icons;

public class Icon : BaseModel
{
	public string Name { get; init; }

	public class IconConfiguration : BaseConfiguration<Icon>
	{
		public override void Configure(EntityTypeBuilder<Icon> builder)
		{
			base.Configure(builder);

			builder
				.HasIndex(i => i.Name)
				.IsUnique();

			builder
				.Property(i => i.Name)
				.IsRequired()
				.HasMaxLength(32);
		}
	}
}