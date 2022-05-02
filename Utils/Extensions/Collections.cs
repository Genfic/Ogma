#region

using System.Collections;

#endregion

namespace Utils.Extensions;

public static class Enumerable
{
	public static string JoinToString(this IEnumerable input, char glue = ' ') => string.Join(glue, input);
	public static string JoinToString(this string?[] input, char glue = ' ') => string.Join(glue, input);
	public static string JoinToString(this IEnumerable input, string glue = "") => string.Join(glue, input);
	public static string JoinToString(this string?[] input, string glue = "") => string.Join(glue, input);
}