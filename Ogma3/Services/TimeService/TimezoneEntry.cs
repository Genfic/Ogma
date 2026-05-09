namespace Ogma3.Services.TimeService;

public sealed record TimezoneEntry(string Value, string Text, TimeSpan Offset) : IComparable<TimezoneEntry>
{
	public int CompareTo(TimezoneEntry? other) => Offset.CompareTo(other?.Offset);
}