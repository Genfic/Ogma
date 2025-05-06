using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogma3.Infrastructure.Extensions;

public static class PropertyBuilderExtensions
{
	public static PropertyBuilder<T> IsCitext<T>(this PropertyBuilder<T> builder) => builder.HasColumnType("citext");
}