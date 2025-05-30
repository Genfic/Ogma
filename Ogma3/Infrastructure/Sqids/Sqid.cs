using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.Extensions.Primitives;
using Sqids;
using StackExchange.Profiling.Internal;

namespace Ogma3.Infrastructure.Sqids;

public readonly record struct Sqid(long Value)
{
	private static readonly SqidsEncoder<long> Sqids = new(new SqidsOptions
	{
		MinLength = 5,
		Alphabet = "7sFf9vItSX4q8pHURbiBZw0EVkxmJ2P3hcuYTCzjadGWOrKe6NLM15onADlQgy",
		BlockList = { "admin" },
	});

	public override string ToString() => Sqids.Encode(Value);

	public static Sqid? Parse(string s)
	{
		if (s.IsNullOrWhiteSpace())
		{
			return null;
		}

		if (Sqids.Decode(s) is [var num] && s == Sqids.Encode(num))
		{
			return new Sqid(num);
		}

		return null;
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? _, out Sqid result)
	{
		if (s.IsNullOrWhiteSpace())
		{
			result = default;
			return false;
		}

		if (Sqids.Decode(s) is [var num] && s == Sqids.Encode(num))
		{
			result = new Sqid(num);
			return true;
		}

		result = default;
		return false;
	}

	[UsedImplicitly]
	public static ValueTask<Sqid?> BindAsync(HttpContext context, ParameterInfo parameter)
	{
		var name = parameter.Name;

		if (name is null)
		{
			throw new InvalidOperationException("Sqid parameters must have a name.");
		}

		// Try from route first
		if (context.Request.RouteValues.TryGetValue(name!, out var routeVal) && routeVal is string routeStr)
		{
			if (string.Equals(routeStr, "null", StringComparison.OrdinalIgnoreCase))
			{
				return ValueTask.FromResult<Sqid?>(null);
			}

			return TryParse(routeStr, null, out var parsedRoute)
				? ValueTask.FromResult<Sqid?>(parsedRoute)
				: throw new FormatException($"Invalid Sqid in route: {routeStr}");

		}

		// Then try from query
		var queryVal = context.Request.Query[name];
		var queryStr = queryVal.ToString();

		if (string.IsNullOrWhiteSpace(queryStr) || string.Equals(queryStr, "null", StringComparison.OrdinalIgnoreCase))
		{
			return ValueTask.FromResult<Sqid?>(null);
		}

		return TryParse(queryStr, null, out var parsedQuery)
			? ValueTask.FromResult<Sqid?>(parsedQuery)
			: throw new FormatException($"Invalid Sqid in query: {queryStr}");

	}

	public sealed class JsonConverter : JsonConverter<Sqid>
	{
		public override Sqid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.GetString() is not {} str)
			{
				throw new JsonException("Incorrect Sqid format.");
			}
			return Parse(str) ?? throw new JsonException("Incorrect Sqid format.");
		}

		public override void Write(Utf8JsonWriter writer, Sqid value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString());
	}

	public sealed class RouteConstraint : IRouteConstraint
	{
		public bool Match(
			HttpContext? httpContext,
			IRouter? route,
			string routeKey,
			RouteValueDictionary values,
			RouteDirection routeDirection
		)
		{
			return values.TryGetValue(routeKey, out var value)
			       && value is string str
			       && TryParse(str, null, out var parsed)
			       && parsed.ToString() == str;
		}
	}
}