namespace Utils.Extensions;

public static class EnumerableExtensions
{
	public static IEnumerable<T> Tap<T>(this IEnumerable<T> elements, Func<T, string> selector)
	{
	#if DEBUG
		(Console.ForegroundColor, Console.BackgroundColor) = (Console.BackgroundColor, Console.ForegroundColor);
		foreach (var element in elements)
		{
			Console.WriteLine($"TAP: {selector(element)}");
			yield return element;
		}
		(Console.ForegroundColor, Console.BackgroundColor) = (Console.BackgroundColor, Console.ForegroundColor);
	#else
		foreach (var element in elements) yield return element;
	#endif
	}

	public static IEnumerable<T> GetValues<TKey, T>(this IEnumerable<IGrouping<TKey, T>> grouping, Func<TKey, bool> predicate)
		=> grouping.Where(g => predicate(g.Key)).SelectMany(x => x);
}