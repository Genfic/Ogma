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
    public static INpgsqlTypeMapper MapPostgresEnums(this INpgsqlTypeMapper mapper, Assembly assembly, INpgsqlNameTranslator? translator = null)
    {
        var enums = GetEnums(assembly);
        if (enums is { Length: <= 0 }) return mapper;
            
        Log.Information("Mapping Postgres Enums:");
        foreach (var type in enums)
        {
            Log.Information("   Mapping {FullName}", type.FullName);

            var name = type.GetCustomAttribute<PostgresEnumAttribute>()?.Name;

            var method = mapper.GetType().GetMethod(nameof(mapper.MapEnum))?.MakeGenericMethod(type);
            if (method is {} m)
            {
                m.Invoke(mapper, new object?[]{ name, translator });
            }
            else
            {
                Log.Warning("           No such method");
            }
        }

        return mapper;
    }

    public static void RegisterPostgresEnums(this ModelBuilder builder, Assembly assembly, string? schema = null, INpgsqlNameTranslator? translator = null)
    {
        var enums = GetEnums(assembly);
        if (enums is { Length: <= 0 }) return;
            
        Log.Information("Registering Postgres Enums:");
        foreach (var type in enums)
        {
            Log.Information("   Registering {FullName}", type.FullName);

            var name = type.GetCustomAttribute<PostgresEnumAttribute>()?.Name;
                
            var method = typeof(NpgsqlModelBuilderExtensions)
                .GetMethods()
                .Where(mi => mi.Name == nameof(NpgsqlModelBuilderExtensions.HasPostgresEnum))
                .Single(mi => mi.IsGenericMethod)
                ?.MakeGenericMethod(type);
                
            if (method is {} m)
            {
                m.Invoke(null, new object?[]{ builder, schema, name, translator });
            }
            else
            {
                Log.Warning("       No such method");
            }
        }
    }

    private static ImmutableArray<Type> GetEnums(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.IsEnum)
            .Where(t => t.GetCustomAttributes(typeof(PostgresEnumAttribute), false) is { Length: > 0 })
            .GroupBy(t => t.FullName)
            .Select(tt => tt.First())
            .ToImmutableArray();
    }
}