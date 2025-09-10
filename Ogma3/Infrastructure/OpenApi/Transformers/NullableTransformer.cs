using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.OpenApi;

// This transformer attempts to coalesce nullable and non-nullable schemas by removing the `nullable` property
// wherever nullability is already implied by the `required` property.
// It also removes `null` from enum values if present.
// Finally, it removes the "NullableOf" prefix from schema reference IDs if present, being careful to preserve
// the original reference ID for non-nullable types.
// Source: https://github.com/mikekistler/aspnet-transformer-gallery/blob/main/TransformerGallery/Transformers/NullableTransformer.cs
namespace Ogma3.Infrastructure.OpenApi.Transformers;

public static partial class NullableTransformer
{
	internal sealed partial class ChainedDelegate(Func<JsonTypeInfo, string?> next) {

		public string? Invoke(JsonTypeInfo type) {
			// Get the result of the next delegate in the chain
			var result = next(type);
			// remove the "NullableOf" prefix for nullable types if present
			if (result is not null && type.Type.IsGenericType && type.Type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
				result = NullableTypeRegex.Replace(result, "");
			}
			return result;
		}

		[GeneratedRegex("^NullableOf")]
		private static partial Regex NullableTypeRegex { get; }
	}

	public static OpenApiOptions AddNullableTransformer(this OpenApiOptions options)
	{
		options.AddSchemaTransformer((schema, _, _) =>
		{
			if (schema.Properties is null) return Task.CompletedTask;

			foreach (var property in schema.Properties)
			{
				// Also need to remove `null` from enum values if present
				if (property.Value.Enum is not null)
				{
					schema.Required?.Add(property.Key);
				}

				// And remove default value of null if set
				// if (property.Value.Default is NullOpenApiString)
				// {
				// 	property.Value.Default = null;
				// }
			}

			return Task.CompletedTask;
		});

		var chainedDelegate = new ChainedDelegate(options.CreateSchemaReferenceId);
		options.CreateSchemaReferenceId = chainedDelegate.Invoke;

		return options;
	}
}