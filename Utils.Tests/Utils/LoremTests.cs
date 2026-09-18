using System.Net;

namespace Utils.Tests.Utils;

public sealed class LoremTests
{
	private sealed class RecordingHandler : HttpMessageHandler
	{
		public Uri? LastRequestUri { get; private set; }

		protected override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			LastRequestUri = request.RequestUri;
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("ok"),
			});
		}
	}

	private static async Task<(Uri Uri, string Body)> RunIpsum(int paragraphs, IpsumOptions? options, RecordingHandler handler)
	{
		Lorem.Client = new HttpClient(handler);
		try
		{
			var body = await Lorem.Ipsum(paragraphs, options);
			return (handler.LastRequestUri!, body);
		}
		finally
		{
			Lorem.Client.Dispose();
			Lorem.Client = new HttpClient();
		}
	}

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

	[Test]
	[NotInParallel("Lorem.Ipsum")]
	public async Task Ipsum_NoOptions()
	{
		var handler = new RecordingHandler();
		var (uri, body) = await RunIpsum(5, null, handler);

		await Assert.That(uri.ToString()).IsEqualTo("https://loripsum.net/api/5");
		await Assert.That(body).IsEqualTo("ok");
	}

	[Test]
	[NotInParallel("Lorem.Ipsum")]
	[Arguments(IpsumLength.Short, "/short")]
	[Arguments(IpsumLength.Medium, "/medium")]
	[Arguments(IpsumLength.Long, "/long")]
	[Arguments(IpsumLength.Verylong, "/verylong")]
	public async Task Ipsum_Lengths(IpsumLength length, string expectedSegment)
	{
		var handler = new RecordingHandler();
		var options = new IpsumOptions(
			Length: length,
			Decorate: false,
			Link: false,
			Ulist: false,
			Olist: false,
			Dlist: false,
			Blockquotes: false,
			Codeblocks: false,
			Headers: false,
			Allcaps: false,
			Prude: false,
			Plaintext: false
		);
		var (uri, _) = await RunIpsum(1, options, handler);

		await Assert.That(uri.ToString()).IsEqualTo($"https://loripsum.net/api/1{expectedSegment}");
	}

	[Test]
	[NotInParallel("Lorem.Ipsum")]
	public async Task Ipsum_AllOptions()
	{
		var handler = new RecordingHandler();
		var options = new IpsumOptions(
			Length: IpsumLength.Short,
			Decorate: true,
			Link: true,
			Ulist: true,
			Olist: true,
			Dlist: true,
			Blockquotes: true,
			Codeblocks: true,
			Headers: true,
			Allcaps: true,
			Prude: true,
			Plaintext: true
		);
		var (uri, _) = await RunIpsum(3, options, handler);

		await Assert.That(uri.ToString()).IsEqualTo(
			"https://loripsum.net/api/3/short/decorate/link/ul/ol/dl/bq/code/headers/allcaps/prude/plaintext");
	}
}