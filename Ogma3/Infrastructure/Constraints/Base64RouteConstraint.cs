using System.Buffers;
using System.Buffers.Text;
using System.Text.RegularExpressions;

namespace Ogma3.Infrastructure.Constraints;

public sealed partial class Base64RouteConstraint : IRouteConstraint
{
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var value) || value is not string s)
            return false;

        if (string.IsNullOrEmpty(s))
            return false;

        // Fast structural validation (generated at compile time)
        if (!Base64Regex().IsMatch(s))
            return false;

        // Normalize URL-safe Base64 inline (minimal allocation)
        var normalized = s.Length <= 256
            ? stackalloc char[s.Length + 2]
            : new char[s.Length + 2];

        var i = 0;
        foreach (var c in s)
        {
            normalized[i++] = c switch
            {
                '-' => '+',
                '_' => '/',
                _ => c
            };
        }

        // Fix padding
        var padding = (4 - (i % 4)) % 4;
        for (var p = 0; p < padding; p++)
            normalized[i++] = '=';

        ReadOnlySpan<char> finalSpan = normalized[..i];

        // Validate decode without throwing
        var utf8 = finalSpan.Length <= 256
            ? stackalloc byte[finalSpan.Length]
            : new byte[finalSpan.Length];

        for (var j = 0; j < finalSpan.Length; j++)
            utf8[j] = (byte)finalSpan[j];

        var output = utf8.Length <= 256
            ? stackalloc byte[Base64.GetMaxDecodedFromUtf8Length(utf8.Length)]
            : new byte[Base64.GetMaxDecodedFromUtf8Length(utf8.Length)];

        var status = Base64.DecodeFromUtf8(utf8, output, out _, out _);

        return status == OperationStatus.Done;
    }

    // Source-generated at compile time
    [GeneratedRegex(@"^[A-Za-z0-9\-_+/]*={0,2}$", RegexOptions.CultureInvariant)]
    private static partial Regex Base64Regex();
}