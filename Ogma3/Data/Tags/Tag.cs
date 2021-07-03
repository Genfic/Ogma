using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;
using Ogma3.Data.Stories;
using Utils.Extensions;

namespace Ogma3.Data.Tags
{
    public class Tag : BaseModel
    {
        private readonly string _name;
        public string Name { 
            get => _name;
            init
            {
                _name = value;
                Slug = value.Friendlify();
            } 
        }
        public string Slug { get; private set; }
        public string? Description { get; init; }
        public ETagNamespace? Namespace { get; init; }
        public IEnumerable<Story> Stories { get; init; }
    }
}