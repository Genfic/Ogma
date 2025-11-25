using System.Collections;

namespace Utils.Extensions;

public static class Enumerable
{
	public static string JoinToString(this IEnumerable input, char glue = ' ') => string.Join(glue, input);
	public static string JoinToString(this string?[] input, char glue = ' ') => string.Join(glue, input);
	public static string JoinToString(this IEnumerable input, string glue = "") => string.Join(glue, input);
	public static string JoinToString(this string?[] input, string glue = "") => string.Join(glue, input);
}

public static class List
{
	/// <summary>
	/// Add multiple items to the IList
	/// </summary>
	/// <param name="list">List to add items to</param>
	/// <param name="items">Items to add</param>
	/// <typeparam name="T">Type of the items</typeparam>
	public static void AddMany<T>(this IList<T> list, params T[] items)
	{
		foreach (var item in items)
		{
			list.Add(item);
		}
	}
}

public static class Dictionary
{
	public static void AddMany<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other, bool replace = true)
	{
		foreach (var (key, value) in other)
		{
			if (replace)
			{
				dict[key] = value;
			}
			else
			{
				_ = dict.TryAdd(key, value);
			}
		}
	}
}