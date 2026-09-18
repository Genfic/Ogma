using System.Buffers.Text;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Ogma3.Infrastructure.Constraints;

namespace Ogma3.Tests.Infrastructure.Constraints;

public sealed class Base64RouteConstraintTest
{
	private const string RouteKey = "id";

	private static bool Match(object? value, bool includeKey = true)
	{
		var values = new RouteValueDictionary();
		if (includeKey)
		{
			values[RouteKey] = value;
		}

		return new Base64RouteConstraint().Match(null, null, RouteKey, values, RouteDirection.IncomingRequest);
	}

	[Test]
	[Arguments("payload")]
	[Arguments("the quick brown fox jumps over the lazy dog")]
	public async Task TestValidBase64UrlMatches(string input)
	{
		var valid = Base64Url.EncodeToString(Encoding.UTF8.GetBytes(input));
		await Assert.That(Match(valid)).IsTrue();
	}

	[Test]
	public async Task TestMissingKeyDoesNotMatch()
	{
		await Assert.That(Match(null, includeKey: false)).IsFalse();
	}

	[Test]
	public async Task TestNullValueDoesNotMatch()
	{
		await Assert.That(Match(null)).IsFalse();
	}

	[Test]
	public async Task TestEmptyStringDoesNotMatch()
	{
		await Assert.That(Match("")).IsFalse();
	}

	[Test]
	public async Task TestNonBase64StringDoesNotMatch()
	{
		await Assert.That(Match("!!!not-base64!!!")).IsFalse();
	}

	[Test]
	public async Task TestNonStringValueDoesNotMatch()
	{
		await Assert.That(Match(12345)).IsFalse();
	}
}