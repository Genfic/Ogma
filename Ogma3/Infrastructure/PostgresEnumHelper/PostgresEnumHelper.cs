#nullable enable


using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.TypeMapping;
using Serilog;

namespace Ogma3.Infrastructure.PostgresEnumHelper;

public static class PostgresEnumHelper
{
	private static Type[] _enums = Array.Empty<Type>();

	public static INpgsqlTypeMapper MapPostgresEnums(this INpgsqlTypeMapper mapper, Assembly assembly,
		INpgsqlNameTranslator? translator = null)
	{
		var enums = GetEnums(assembly);
		if (enums is { Length: <= 0 }) return mapper;

		Log.Information("Mapping Postgres Enums:");

		var methodType = mapper.GetType().GetMethod(nameof(mapper.MapEnum));

		if (methodType is null)
		{
			Log.Warning("No {Method Name} method found", nameof(NpgsqlModelBuilderExtensions.HasPostgresEnum));
			return mapper;
		}

		foreach (var type in enums)
		{
			Log.Information("   Mapping {FullName}", type.FullName);

			var name = type.GetCustomAttribute<PostgresEnumAttribute>()?.Name;

			var method = methodType?.MakeGenericMethod(type);

			if (method is { } m)
			{
				m.Invoke(mapper, new object?[] { name, translator });
			}
		}

		return mapper;
	}

	public static void RegisterPostgresEnums(this ModelBuilder builder, Assembly assembly, string? schema = null,
		INpgsqlNameTranslator? translator = null)
	{
		var enums = GetEnums(assembly);
		if (enums is { Length: <= 0 }) return;

		Log.Information("Registering Postgres Enums:");

		var methodType = typeof(NpgsqlModelBuilderExtensions)
			.GetMethods()
			.Where(mi => mi.Name == nameof(NpgsqlModelBuilderExtensions.HasPostgresEnum))
			.SingleOrDefault(mi => mi.IsGenericMethod);

		if (methodType is null)
		{
			Log.Warning("   No {Method Name} method found", nameof(NpgsqlModelBuilderExtensions.HasPostgresEnum));
			return;
		}

		foreach (var type in enums)
		{
			Log.Information("   Registering {FullName}", type.FullName);

			var name = type.GetCustomAttribute<PostgresEnumAttribute>()?.Name;

			var method = methodType.MakeGenericMethod(type);

			if (method is { } m)
			{
				m.Invoke(null, new object?[] { builder, schema, name, translator });
			}
		}
	}

	private static Type[] GetEnums(Assembly assembly)
	{
		if (_enums.Length > 0) return _enums;

		Log.Warning($"{nameof(PostgresEnumHelper)} cache miss");
		_enums = assembly.GetTypes()
			.Where(t => t.IsEnum)
			.Where(t => t.GetCustomAttributes(typeof(PostgresEnumAttribute), false) is { Length: > 0 })
			.GroupBy(t => t.FullName)
			.Select(tt => tt.First())
			.ToArray();

		return _enums;
	}
}