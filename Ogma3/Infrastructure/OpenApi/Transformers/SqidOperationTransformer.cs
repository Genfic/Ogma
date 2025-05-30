using System.Text.RegularExpressions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Ogma3.Infrastructure.Sqids;

namespace Ogma3.Infrastructure.OpenApi.Transformers;

public sealed partial class SqidOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
	{

		if (context.Description.RelativePath is { } path)
		{
			var matches = MyRegex().Matches(path);

			foreach (var match in matches.Cast<Match>())
			{
				var name = match.Groups["name"].Value;

				if (operation.Parameters?.Any(p => p.Name == name) ?? false)
				{
					continue;
				}

				operation.Parameters?.Add(new OpenApiParameter
				{
					Name = name,
					In = ParameterLocation.Path,
					Required = true,
					Schema = new OpenApiSchema()
					{
						Type = "string",
						Format = "sqid",
						Description = "SQID-encoded value",
					}
				});
			}
		}

		foreach (var parameter in operation.Parameters ?? [])
		{
			if (parameter.Schema?.Reference?.Id is {} id && (id == nameof(Sqid) || id.EndsWith(nameof(Sqid))))
			{
				parameter.Schema.Type = "string";
				parameter.Schema.Format = "sqid";
				parameter.Schema.Description = "SQID-encoded value";
				parameter.Schema.Reference = null;
				parameter.Schema.Properties.Clear();
				parameter.Schema.Required.Clear();
			}
		}

		return Task.CompletedTask;
	}

	[GeneratedRegex(@"\{(?<name>[^:}]+):sqid\}")]
	private static partial Regex MyRegex();
}