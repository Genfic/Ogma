using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ogma3.Data.Enums;
using Utils.Extensions;

namespace Ogma3.Data.Models
{
    public class Tag : BaseModel
    {
        private string _name;
        
        [MinLength(CTConfig.CTag.MinNameLength)]
        public string Name { 
            get => _name;
            set
            {
                _name = value;
                Slug = value.Friendlify();
            } 
        }

        [MinLength(CTConfig.CTag.MinNameLength)]
        public string Slug { get; private set; }
        public string? Description { get; set; } = null;
        public ETagNamespace? Namespace { get; set; }
        public IEnumerable<Story> Stories { get; set; }
    }
}