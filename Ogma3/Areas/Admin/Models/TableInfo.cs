using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogma3.Areas.Admin.Models;

public class TableInfo
{
    public string Name { get; set; }
    public int Size { get; set; }
    
    public class Configuration : IEntityTypeConfiguration<TableInfo>
    {
        public void Configure(EntityTypeBuilder<TableInfo> builder)
        {
            builder.HasNoKey();
        }
    }
}