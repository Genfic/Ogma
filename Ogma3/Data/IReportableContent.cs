using Ogma3.Data.Reports;

namespace Ogma3.Data;

public interface IReportableContent
{
	public ICollection<Report> Reports { get; set; }
}