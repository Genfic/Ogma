using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Ogma3.Infrastructure.IResults;

namespace Ogma3.Tests.Infrastructure.IResults;

public sealed class UnauthorizedResultTest
{
	private static DefaultHttpContext CreateContext()
	{
		var context = new DefaultHttpContext
		{
			RequestServices = new ServiceCollection()
				.AddLogging()
				.BuildServiceProvider(),
		};
		return context;
	}

	[Test]
	public async Task TestExecuteAsyncSetsStatus401()
	{
		var context = CreateContext();
		var result = new UnauthorizedResult();

		await result.ExecuteAsync(context);

		await Assert.That(context.Response.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);
	}

	[Test]
	public async Task TestExecuteAsyncWithNullContextThrows()
	{
		var result = new UnauthorizedResult();

		await Assert.That(() => result.ExecuteAsync(null!)).Throws<ArgumentNullException>();
	}

	[Test]
	public async Task TestPopulateMetadataAdds401ResponseMetadata()
	{
		var builder = new TestEndpointBuilder();
		UnauthorizedResult.PopulateMetadata(
			typeof(UnauthorizedResult).GetMethod(nameof(UnauthorizedResult.ExecuteAsync))!,
			builder
		);

		await Assert.That(builder.Metadata.Count).IsEqualTo(1);
		var metadata = (ProducesResponseTypeMetadata)builder.Metadata[0];
		await Assert.That(metadata.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);
		await Assert.That(metadata.ContentTypes).IsEquivalentTo(["application/json"]);
	}

	private sealed class TestEndpointBuilder : EndpointBuilder
	{
		public override Endpoint Build() => throw new NotImplementedException();
	}
}