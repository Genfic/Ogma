namespace Ogma3.Data.Bases;

public interface ISoftDeletableContent
{
	DateTimeOffset? ScheduledForDeletion { get; set; }
}