using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Ogma3.Infrastructure.IResults;

namespace Ogma3.Tests.Infrastructure.IResults;

public sealed class NotModifiedResultTest
{
	[Test]
	public async Task TestExecuteAsyncSetsStatus304()
	{
		var context = new DefaultHttpContext();
		var result = new NotModifiedResult();

		await result.ExecuteAsync(context);

		await Assert.That(context.Response.StatusCode).IsEqualTo(StatusCodes.Status304NotModified);
	}

	[Test]
	public async Task TestPopulateMetadataAdds304ResponseMetadata()
	{
		var builder = new TestEndpointBuilder();
		NotModifiedResult.PopulateMetadata(MethodInfo(), builder);

		await Assert.That(builder.Metadata.Count).IsEqualTo(1);
		var metadata = (ProducesResponseTypeMetadata)builder.Metadata[0];
		await Assert.That(metadata.StatusCode).IsEqualTo(StatusCodes.Status304NotModified);
		await Assert.That(metadata.ContentTypes).IsEquivalentTo(["application/json"]);
	}

	private static MethodInfo MethodInfo() =>
		typeof(NotModifiedResult).GetMethod(nameof(NotModifiedResult.ExecuteAsync))!;

	private sealed class TestEndpointBuilder : EndpointBuilder
	{
		public override Endpoint Build() => throw new NotImplementedException();
	}
}