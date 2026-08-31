namespace Utils.Extensions;

public static class Span
{
	public static int IndexOfBefore<T>(this ReadOnlySpan<T> span, T item, int index) where T : IEquatable<T>
		=> span[..(index + 1)].LastIndexOf(item);

	public static int IndexOfAfter<T>(this ReadOnlySpan<T> span, T item, int index) where T : IEquatable<T>
	{
		var idx = span[index..].IndexOf(item);
		return idx < 0
			? -1
			: idx + index;
	}

}