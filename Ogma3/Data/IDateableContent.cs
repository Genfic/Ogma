namespace Ogma3.Data;

public interface IDateableContent
{
	public DateTimeOffset? PublicationDate { get; set; }
	public DateTimeOffset CreationDate { get; set; }
}