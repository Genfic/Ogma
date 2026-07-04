namespace Utils.Extensions;

public static class Enumerable
{
	public static string JoinToString(this IEnumerable<string> input, char glue = ' ') => string.Join(glue, input);
	public static string JoinToString(this string?[] input, char glue = ' ') => string.Join(glue, input);
	public static string JoinToString(this IEnumerable<string> input, string glue = "") => string.Join(glue, input);
	public static string JoinToString(this string?[] input, string glue = "") => string.Join(glue, input);

	public static IEnumerable<(int key, T value)> Keyed<T>(this IEnumerable<T> input)
	{
		var i = 0;
		foreach (var item in input)
		{
			yield return (i++, item);
		}
	}
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
	extension<TKey, TValue>(IDictionary<TKey, TValue> dict)
	{
		public void AddMany(IDictionary<TKey, TValue> other, bool replace = true)
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

		public TValue GetOrDefault(TKey key, TValue defaultValue = default!)
		{
			return dict.TryGetValue(key, out var value) ? value : defaultValue;
		}
	}
}