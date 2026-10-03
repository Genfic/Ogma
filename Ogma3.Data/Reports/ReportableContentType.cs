using NetEscapades.EnumGenerators;

namespace Ogma3.Data.Reports;

[EnumExtensions]
public enum ReportableContentType : short
{
	Comment,
	User,
	Story,
	Chapter,
	Blogpost,
	Club,
}