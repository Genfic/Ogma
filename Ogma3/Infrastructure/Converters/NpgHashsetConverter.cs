using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ogma3.Infrastructure.Converters;

public class NpgHashsetConverter<T>() : ValueConverter<HashSet<T>, List<T>>(
	hs => new List<T>(hs),
	ls => new HashSet<T>(ls)
);