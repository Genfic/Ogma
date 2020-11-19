using System.Collections.Generic;
using Ogma3.Data.Models;

namespace Ogma3.Infrastructure.Comparers
{
    public class OgmaRoleComparer : IEqualityComparer<OgmaRole>
    {
        public bool Equals(OgmaRole x, OgmaRole y)
        {
            if (ReferenceEquals(x, y)) 
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            
            return x.Id == y.Id;
        }

        public int GetHashCode(OgmaRole obj)
        {
            var hashId = obj.Id.GetHashCode();
            var hashName = obj.Name.GetHashCode();
            
            return hashId ^ hashName;
        }
    }
}