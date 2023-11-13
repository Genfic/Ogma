#nullable enable

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;

namespace Ogma3.Infrastructure.PostgresEnumHelper;

public static class PostgresEnumHelper
{
	private static ImmutableList<Type>? _enums;
	private static MethodInfo? _mapMethod;
	private static MethodInfo? _registerMethod;

	/// <summary>
	/// Maps found enums marked with <see cref="PostgresEnumAttribute"/> enum and maps database enum types to them.
	/// This is done only once, on startup.
	/// See https://www.npgsql.org/efcore/mapping/enum.html#mapping-your-enum
	/// </summary>
	/// <param name="builder">Data source builder.</param>
	/// <param name="assembly">Assembly containing the enums.</param>
	/// <param name="translator">Optional translator between database enum types and CLR enum types.</param>
	/// <returns>The configured builder.</returns>
	public static NpgsqlDataSourceBuilder MapPostgresEnums(
		this NpgsqlDataSourceBuilder builder, 
		Assembly assembly,
		INpgsqlNameTranslator? translator = null
	) {
		var enums = GetEnums(assembly);
		if (enums is { Count: <= 0 }) return builder;

		Log.Information("Mapping Postgres Enums:");

		_mapMethod ??= builder.GetType().GetMethod(nameof(builder.MapEnum));

		if (_mapMethod is null)
		{
			Log.Warning("No {MethodName} method found", nameof(builder.MapEnum));
			return builder;
		}

		foreach (var type in enums)
		{
			Log.Information("   Mapping {FullName}", type.FullName);

			var name = type.GetCustomAttribute<PostgresEnumAttribute>()?.Name;

			_mapMethod
				?.MakeGenericMethod(type)
				.Invoke(builder, new object?[] { name, translator });
		}

		return builder;
	}

	/// <summary>
	/// Registers enums marked with <see cref="PostgresEnumAttribute"/> enum with EF Core.
	/// This is done multiple times during runtime, every time a new context gets instantiated.
	/// See https://www.npgsql.org/efcore/mapping/enum.html#creating-your-database-enum
	/// </summary>
	/// <param name="builder">Data source builder.</param>
	/// <param name="assembly">Assembly containing the enums.</param>
	/// <param name="schema">The schema in which to create the enum types.</param>
	/// <param name="translator">Optional translator between database enum types and CLR enum types.</param>
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
			Log.Warning("   No {MethodName} method found", nameof(NpgsqlModelBuilderExtensions.HasPostgresEnum));
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