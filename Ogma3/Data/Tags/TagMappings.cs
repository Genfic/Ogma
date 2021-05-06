using System;
using System.Linq.Expressions;

namespace Ogma3.Data.Tags
{
    public static class TagMappings
    {
        public static readonly Expression<Func<Tag, TagDto>> ToTagDto = t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
            Slug = t.Slug,
            Namespace = t.Namespace,
            Description = t.Description
        };
    }
}