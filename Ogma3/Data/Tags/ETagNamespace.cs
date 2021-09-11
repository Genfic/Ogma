using System.ComponentModel.DataAnnotations;
using Ogma3.Infrastructure.PostgresEnumHelper;

namespace Ogma3.Data.Tags
{
    [PostgresEnum]
    public enum ETagNamespace
    {
        [Display(Name = "Content Warning")]
        ContentWarning = 1,
        Genre = 2,
        Franchise = 3,
    }

    public static class ETagNamespaceExtensions
    {
        public static string GetColor(this ETagNamespace? ns) => ns switch
        {
            ETagNamespace.ContentWarning => "#d91919",
            ETagNamespace.Genre => "#8c37f4",
            ETagNamespace.Franchise => "#18f900",
            _ => "transparent"
        };
    }
}