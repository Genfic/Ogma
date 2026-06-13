namespace Utils.Tests.Utils;

public sealed class LoremTests
{
	[Test]
	public async Task Picsum_BasicWidth()
	{
		var result = Lorem.Picsum(800);
		
		await Assert.That(result).IsEqualTo("//picsum.photos/800");
	}

	[Test]
	public async Task Picsum_WidthAndHeight()
	{
		var result = Lorem.Picsum(800, 600);
		
		await Assert.That(result).IsEqualTo("//picsum.photos/800/600");
	}

	[Test]
	public async Task Picsum_WithNullHeight()
	{
		var result = Lorem.Picsum(800, null);
		
		await Assert.That(result).IsEqualTo("//picsum.photos/800");
	}

	// Note: These tests require network access to loripsum.net
	// They are commented out by default. Uncomment and mock HttpClient for proper testing.
	// [Test]
	// public async Task Ipsum_BasicParagraphs()
	// {
	// 	var result = await Lorem.Ipsum(1, null);
	// 	await Assert.That(result).IsNotNull();
	// 	await Assert.That(result).IsNotEmpty();
	// }

	// [Test]
	// public async Task Ipsum_MultipleParagraphs()
	// {
	// 	var result = await Lorem.Ipsum(3, null);
	// 	await Assert.That(result).IsNotNull();
	// 	await Assert.That(result).IsNotEmpty();
	// }

	// Note: These tests require network access to loripsum.net and are commented out
	//[Test]
	//public async Task Ipsum_WithOptions()
	//{
	//	var options = new IpsumOptions(
	//		Length: IpsumLength.Short,
	//		Decorate: true,
	//		Link: true,
	//		Ulist: false,
	//		Olist: false,
	//		Dlist: false,
	//		Blockquotes: false,
	//		Codeblocks: false,
	//		Headers: false,
	//		Allcaps: false,
	//		Prude: false,
	//		Plaintext: false
	//	);
	//	
	//	var result = await Lorem.Ipsum(1, options);
	//	
	//	await Assert.That(result).IsNotNull();
	//	await Assert.That(result).Contains("/short");
	//	await Assert.That(result).Contains("/decorate");
	//	await Assert.That(result).Contains("/link");
	//}

	//[Test]
	//public async Task Ipsum_WithAllOptions()
	//{
	//	var options = new IpsumOptions(
	//		Length: IpsumLength.Medium,
	//		Decorate: true,
	//		Link: true,
	//		Ulist: true,
	//		Olist: true,
	//		Dlist: true,
	//		Blockquotes: true,
	//		Codeblocks: true,
	//		Headers: true,
	//		Allcaps: true,
	//		Prude: true,
	//		Plaintext: true
	//	);
	//	
	//	var result = await Lorem.Ipsum(1, options);
	//	
	//	await Assert.That(result).IsNotNull();
	//	await Assert.That(result).Contains("/medium");
	//	await Assert.That(result).Contains("/decorate");
	//	await Assert.That(result).Contains("/link");
	//	await Assert.That(result).Contains("/ul");
	//	await Assert.That(result).Contains("/ol");
	//	await Assert.That(result).Contains("/dl");
	//	await Assert.That(result).Contains("/bq");
	//	await Assert.That(result).Contains("/code");
	//	await Assert.That(result).Contains("/headers");
	//	await Assert.That(result).Contains("/allcaps");
	//	await Assert.That(result).Contains("/prude");
	//	await Assert.That(result).Contains("/plaintext");
	//}

	//[Test]
	//public async Task Ipsum_WithNullOptions()
	//{
	//	var result = await Lorem.Ipsum(1, null);
	//	
	//	await Assert.That(result).IsNotNull();
	//	await Assert.That(result).DoesNotContain("/");
	//}

	//[Test]
	//[Arguments(IpsumLength.Short)]
	//[Arguments(IpsumLength.Medium)]
	//[Arguments(IpsumLength.Long)]
	//[Arguments(IpsumLength.Verylong)]
	//public async Task Ipsum_AllLengths(IpsumLength length)
	//{
	//	var options = new IpsumOptions(
	//		Length: length,
	//		Decorate: false,
	//		Link: false,
	//		Ulist: false,
	//		Olist: false,
	//		Dlist: false,
	//		Blockquotes: false,
	//		Codeblocks: false,
	//		Headers: false,
	//		Allcaps: false,
	//		Prude: false,
	//		Plaintext: false
	//	);
	//	var result = await Lorem.Ipsum(1, options);
	//	
	//	await Assert.That(result).Contains($"/{length.ToStringFast().ToLower()}");
	//}
}
