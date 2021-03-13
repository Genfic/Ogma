using System.ComponentModel;

namespace Ogma3.Data.Enums
{
    public enum ETagNamespace
    {
        [Description("Content Warning")]
        ContentWarning,
        Genre,
        Franchise,
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