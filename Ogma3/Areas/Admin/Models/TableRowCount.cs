using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogma3.Areas.Admin.Models;

public class TableRowCount
{
    public string Name { get; init; }
    public int Count { get; init; }
    
    public class Configuration : IEntityTypeConfiguration<TableRowCount>
    {
        public void Configure(EntityTypeBuilder<TableRowCount> builder) => builder.HasNoKey();
    }
}