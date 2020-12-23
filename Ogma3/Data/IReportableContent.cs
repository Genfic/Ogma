using System.Collections.Generic;
using Ogma3.Data.Models;

namespace Ogma3.Data
{
    public interface IReportableContent
    {
        public ICollection<Report> Reports { get; set; }
    }
}