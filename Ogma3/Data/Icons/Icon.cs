#nullable disable

using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Icons;

[AutoDbSet]
public sealed class Icon : BaseModel
{
	public string Name { get; init; }

	public sealed class IconConfiguration : BaseConfiguration<Icon>
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