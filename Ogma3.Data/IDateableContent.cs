namespace Ogma3.Data;

public interface IDateableContent
{
	DateTimeOffset? PublicationDate { get; set; }
	DateTimeOffset CreationDate { get; set; }
}