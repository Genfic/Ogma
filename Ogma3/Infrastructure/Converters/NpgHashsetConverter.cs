using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.ValueConversion;

namespace Ogma3.Infrastructure.Converters;

public class NpgHashsetConverter<T> : ValueConverter<HashSet<T>, List<T>>, INpgsqlArrayConverter
{
	public NpgHashsetConverter() : base(
		hs => new List<T>(hs),
		ls => new HashSet<T>(ls)
	)
	{
	}

	public ValueConverter ElementConverter => new ValueConverter<T, T>(x => x, x => x);
}