#nullable enable

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.TypeMapping;
using Serilog;

namespace Ogma3.Infrastructure.PostgresEnumHelper;

public static class PostgresEnumHelper
{
	private static ImmutableList<Type>? _enums;
	private static MethodInfo? _mapMethod;
	private static MethodInfo? _registerMethod;

	public static INpgsqlTypeMapper MapPostgresEnums(
		this INpgsqlTypeMapper mapper, 
		Assembly assembly,
		INpgsqlNameTranslator? translator = null
	) {
		var enums = GetEnums(assembly);
		if (enums is { Count: <= 0 }) return mapper;

		Log.Information("Mapping Postgres Enums:");

		_mapMethod ??= mapper.GetType().GetMethod(nameof(mapper.MapEnum));

		if (_mapMethod is null)
		{
			Log.Warning("No {Method Name} method found", nameof(mapper.MapEnum));
			return mapper;
		}

		foreach (var type in enums)
		{
			Log.Information("   Mapping {FullName}", type.FullName);

			var name = type.GetCustomAttribute<PostgresEnumAttribute>()?.Name;

			_mapMethod
				?.MakeGenericMethod(type)
				.Invoke(mapper, new object?[] { name, translator });
		}

		return mapper;
	}

	public static void RegisterPostgresEnums(
		this ModelBuilder builder,
		Assembly assembly,
		string? schema = null,
		INpgsqlNameTranslator? translator = null
	)
	{
		var enums = GetEnums(assembly);
		if (enums is { Count: <= 0 }) return;

		Log.Information("Registering Postgres Enums:");

		_registerMethod ??= typeof(NpgsqlModelBuilderExtensions)
			.GetMethods()
			.Where(mi => mi.Name == nameof(NpgsqlModelBuilderExtensions.HasPostgresEnum))
			.SingleOrDefault(mi => mi.IsGenericMethod);

		if (_registerMethod is null)
		{
			Log.Warning("   No {Method Name} method found", nameof(NpgsqlModelBuilderExtensions.HasPostgresEnum));
			return;
		}

		foreach (var type in enums)
		{
			Log.Information("   Registering {FullName}", type.FullName);

			var name = type.GetCustomAttribute<PostgresEnumAttribute>()?.Name;

			_registerMethod
				?.MakeGenericMethod(type)
				.Invoke(null, new object?[] { builder, schema, name, translator });
		}
	}

	private static ImmutableList<Type> GetEnums(Assembly assembly)
	{
		if (_enums is { Count: > 0 }) return _enums;

		Log.Warning($"{nameof(PostgresEnumHelper)} cache miss");
		
		_enums = assembly.GetTypes()
			.Where(t => t.IsEnum)
			.Where(t => t.IsDefined(typeof(PostgresEnumAttribute), false))
			.DistinctBy(t => t.FullName)
			.ToImmutableList();

		return _enums;
	}
}