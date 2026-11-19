namespace Utils.Extensions;

public static class TimeZoneInfoExtensions
{
	extension(TimeZoneInfo tzi)
	{
		public string IanaId => TimeZoneInfo.TryConvertWindowsIdToIanaId(tzi.Id, out var iana)
			? iana
			: tzi.Id;
	}
}