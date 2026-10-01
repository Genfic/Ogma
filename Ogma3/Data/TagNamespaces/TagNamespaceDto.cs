namespace Ogma3.Data.TagNamespaces;

public sealed record TagNamespaceDto
(
	long Id,
	string Name,
	string Slug,
	string? Alias,
	string? Color,
	string? Description,
	int TagCount
);

public static class TagNamespaceMapper
{
	public static TagNamespaceDto ToDto(this TagNamespace tns)
		=> new(
			tns.Id,
			tns.Name,
			tns.Slug,
			tns.Alias,
			tns.Color,
			tns.Description,
			tns.Tags.Count
		);

	public static IQueryable<TagNamespaceDto> ProjectToDto(this IQueryable<TagNamespace> query)
		=> query.Select(ns => new TagNamespaceDto(
			ns.Id,
			ns.Name,
			ns.Slug,
			ns.Alias,
			ns.Color,
			ns.Description,
			ns.Tags.Count
		));
}
