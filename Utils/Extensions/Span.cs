namespace Utils.Extensions;

public static class Span
{
	public static int IndexOfBefore<T>(this ReadOnlySpan<T> span, T item, int index) where T : IEquatable<T>
	{
		for (var i = index; i > 0; i--)
		{
			if (span[i].Equals(item))
			{
				return i;
			}
		}

		return -1;
	}

	public static int IndexOfAfter<T>(this ReadOnlySpan<T> span, T item, int index) where T : IEquatable<T>
	{
		for (var i = 0; i <= index; i++)
		{
			if (span[i].Equals(item))
			{
				return i;
			}
		}

		return -1;
	}
}