using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogma3.Data.Bases;

public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseModel
{
	protected abstract void Config(EntityTypeBuilder<TEntity> builder);

	public void Configure(EntityTypeBuilder<TEntity> builder)
	{
		builder
			.HasKey(bm => bm.Id);

		builder
			.Property(bm => bm.Id)
			.IsRequired()
			.ValueGeneratedOnAdd();

		Config(builder);
	}
}