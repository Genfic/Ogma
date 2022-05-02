using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogma3.Areas.Admin.Models;

[Keyless]
public class TableInfo
{
	public string Name { get; init; }
	public int Size { get; init; }

	public class Configuration : IEntityTypeConfiguration<TableInfo>
	{
		public void Configure(EntityTypeBuilder<TableInfo> builder) => builder.HasNoKey().ToView(null);
	}
}