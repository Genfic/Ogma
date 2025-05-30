using System.Diagnostics.CodeAnalysis;
using Sqids;

namespace Ogma3.Services;

public sealed class SqidService
{
	private readonly SqidsEncoder<long> _encoder = new (new SqidsOptions
	{
		MinLength = 5,
		Alphabet = "7sFf9vItSX4q8pHURbiBZw0EVkxmJ2P3hcuYTCzjadGWOrKe6NLM15onADlQgy",
		BlockList = { "admin" },
	});

	public bool TryDecodeSingle(ReadOnlySpan<char> value, [NotNullWhen(true)]out long? sqid)
	{
		if (value.IsEmpty || value.IsWhiteSpace())
		{
			sqid = null;
			return false;
		}

		if (_encoder.Decode(value) is [var num] && value == _encoder.Encode(num))
		{
			sqid = num;
			return true;
		}

		sqid = null;
		return false;
	}

	public long? DecodeSingle(ReadOnlySpan<char> value) 	{
		if (value.IsEmpty || value.IsWhiteSpace())
		{
			return null;
		}

		if (_encoder.Decode(value) is [var num] && value == _encoder.Encode(num))
		{
			return num;
		}

		return null;
	}

	public string EncodeSingle(long sqid) => _encoder.Encode(sqid);

}