using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;

namespace Ogma3.Infrastructure.PostgresEnumHelper;

public static class PostgresEnumHelper
{
	private static ImmutableList<Type>? _enums;
	private static MethodInfo? _mapMethod;

	private static HasPostgresEnumDelegate? _registerDelegate;

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
	)
	{
		var logger = Log.ForContext(typeof(PostgresEnumHelper));
		
		var enums = GetEnums(assembly);
		if (enums is { Count: <= 0 }) return builder;

		logger.Information("Mapping Postgres Enums:");

		_mapMethod ??= builder.GetType().GetMethod(nameof(builder.MapEnum));

		if (_mapMethod is null)
		{
			logger.Warning("    No {MethodName} method found", nameof(builder.MapEnum));
			return builder;
		}

		foreach (var type in enums)
		{
			logger.Information("    Mapping {FullName} enum", type.FullName);

			var name = type.GetCustomAttribute<PostgresEnumAttribute>()?.Name;

			_mapMethod
				?.MakeGenericMethod(type)
				.Invoke(builder, [name, translator]);
		}

		return builder;
	}

	private delegate ModelBuilder HasPostgresEnumDelegate(ModelBuilder builder, string? schema, string? name, string[] labels);

	/// <summary>
	/// Registers enums marked with <see cref="PostgresEnumAttribute"/> enum with EF Core.
	/// This is done multiple times during runtime, every time a new context gets instantiated.
	/// See https://www.npgsql.org/efcore/mapping/enum.html#creating-your-database-enum
	/// </summary>
	/// <param name="builder">Data source builder.</param>
	/// <param name="assembly">Assembly containing the enums.</param>
	/// <param name="schema">The schema in which to create the enum types.</param>
	public static void RegisterPostgresEnums(
		this ModelBuilder builder,
		Assembly assembly,
		string? schema = null
	)
	{
		var logger = Log.ForContext(typeof(PostgresEnumHelper));
		
		var enums = GetEnums(assembly);
		if (enums is { Count: <= 0 }) return;

		logger.Information("Registering Postgres Enums:");

		if (_registerDelegate is null)
		{
			logger.Information("   Register delegate not found in cache");
			
			var mi = typeof(NpgsqlModelBuilderExtensions)
				.GetMethods()
				.Where(mi => mi.Name == nameof(NpgsqlModelBuilderExtensions.HasPostgresEnum))
				.Where(mi => !mi.IsGenericMethod)
				.SingleOrDefault(mi => mi.GetParameters().Length == 4);

			if (mi is null)
			{
				logger.Error("    ⚠ No {MethodName} method found when mapping Postgres enums", nameof(NpgsqlModelBuilderExtensions.HasPostgresEnum));
				return;
			}

			var parameters = mi.GetParameters().Select(param => Expression.Parameter(param.ParameterType, param.Name)).ToList();
			var call = Expression.Call(mi, parameters);
			var del = Expression.Lambda<HasPostgresEnumDelegate>(call, parameters).Compile();

			_registerDelegate = del;
		}


		foreach (var type in enums)
		{
			logger.Information("   Registering {FullName}", type.FullName);

			var name = type.GetCustomAttribute<PostgresEnumAttribute>()?.Name ?? type.Name;
			var labels = Enum.GetNames(type);

			_registerDelegate.Invoke(builder, schema, name, labels);
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