using Ogma3.Data.Reports;

namespace Ogma3.Data;

public interface IReportableContent
{
	List<Report> Reports { get; set; }
}