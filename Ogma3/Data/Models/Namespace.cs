using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Namespace : BaseModel
    {
        [MinLength(CTConfig.CNamespace.MinNameLength)]
        public string Name { get; set; }

        [MinLength(7)]
        public string Color { get; set; }

        public uint? Order { get; set; }
    }
}