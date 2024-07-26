namespace Ogma3.Data;

public interface IDateableContent
{
	public DateTime? PublicationDate { get; set; }
	public DateTime CreationDate { get; set; }
}