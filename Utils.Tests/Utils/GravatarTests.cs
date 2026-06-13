namespace Utils.Tests.Utils;

public sealed class GravatarTests
{
	[Test]
	public async Task Generate_BasicEmail()
	{
		var email = "test@example.com";
		var result = Gravatar.Generate(email);
		
		await Assert.That(result).IsNotNull();
		await Assert.That(result).StartsWith("https://www.gravatar.com/avatar/");
		await Assert.That(result).DoesNotContain("?");
	}

	[Test]
	public async Task Generate_EmailWithWhitespace()
	{
		var email = "  test@example.com  ";
		var result = Gravatar.Generate(email);
		
		var expected = Gravatar.Generate("test@example.com");
		await Assert.That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task Generate_EmailCaseInsensitive()
	{
		var email1 = "Test@Example.COM";
		var email2 = "test@example.com";
		
		var result1 = Gravatar.Generate(email1);
		var result2 = Gravatar.Generate(email2);
		
		await Assert.That(result1).IsEqualTo(result2);
	}

	[Test]
	public async Task Generate_WithDefaultImage()
	{
		var options = new Gravatar.Options(Default: "mp");
		var result = Gravatar.Generate("test@example.com", options);
		
		await Assert.That(result).Contains("d=mp");
	}

	[Test]
	public async Task Generate_WithForceDefault()
	{
		var options = new Gravatar.Options(ForceDefault: true);
		var result = Gravatar.Generate("test@example.com", options);
		
		await Assert.That(result).Contains("f=y");
	}

	[Test]
	public async Task Generate_WithRating()
	{
		var options = new Gravatar.Options(Rating: Gravatar.Ratings.PG);
		var result = Gravatar.Generate("test@example.com", options);
		
		await Assert.That(result).Contains("r=pg");
	}

	[Test]
	public async Task Generate_WithMultipleOptions()
	{
		var options = new Gravatar.Options(
			Default: "identicon",
			ForceDefault: true,
			Rating: Gravatar.Ratings.G
		);
		var result = Gravatar.Generate("test@example.com", options);
		
		await Assert.That(result).Contains("d=identicon");
		await Assert.That(result).Contains("f=y");
		await Assert.That(result).Contains("r=g");
		await Assert.That(result).Contains("?");
	}

	[Test]
	public async Task Generate_WithNullOptions()
	{
		var result = Gravatar.Generate("test@example.com", null);
		
		var expected = Gravatar.Generate("test@example.com");
		await Assert.That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(Gravatar.Ratings.G)]
	[Arguments(Gravatar.Ratings.PG)]
	[Arguments(Gravatar.Ratings.R)]
	[Arguments(Gravatar.Ratings.X)]
	public async Task Generate_AllRatings(Gravatar.Ratings rating)
	{
		var options = new Gravatar.Options(Rating: rating);
		var result = Gravatar.Generate("test@example.com", options);
		
		await Assert.That(result).Contains($"r={rating.ToStringFast().ToLower()}");
	}

	[Test]
	[Arguments(Gravatar.AvatarGenMethods.None)]
	[Arguments(Gravatar.AvatarGenMethods.MysteryPerson)]
	[Arguments(Gravatar.AvatarGenMethods.Identicon)]
	[Arguments(Gravatar.AvatarGenMethods.MonsterId)]
	[Arguments(Gravatar.AvatarGenMethods.Wavatar)]
	[Arguments(Gravatar.AvatarGenMethods.Retro)]
	[Arguments(Gravatar.AvatarGenMethods.Robohash)]
	[Arguments(Gravatar.AvatarGenMethods.Blank)]
	public async Task Generate_AllDefaultImages(string defaultImage)
	{
		var options = new Gravatar.Options(Default: defaultImage);
		var result = Gravatar.Generate("test@example.com", options);
		
		await Assert.That(result).Contains($"d={defaultImage}");
	}
}