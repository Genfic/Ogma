using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Sqids;

namespace Ogma3.Infrastructure.Squids;

public static class Squid
{
	private static readonly SqidsEncoder<long> Encoder = new (new SqidsOptions
	{
		MinLength = 5,
		Alphabet = "7sFf9vItSX4q8pHURbiBZw0EVkxmJ2P3hcuYTCzjadGWOrKe6NLM15onADlQgy",
		BlockList = { "admin" },
	});

	public static IServiceCollection UseSqids(this IServiceCollection services) => services.AddSingleton(Encoder);

	public static bool TryDecodeSingle<T>(this SqidsEncoder<T> encoder, ReadOnlySpan<char> value, [NotNullWhen(true)]out T? sqid)
		where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
	{
		if (value.IsEmpty || value.IsWhiteSpace())
		{
			sqid = null;
			return false;
		}
		
		if (encoder.Decode(value) is [var num] && value == encoder.Encode(num))
		{
			sqid = num;
			return true;
		}
		
		sqid = null;
		return false;
	}
}