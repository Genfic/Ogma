using Ogma3.Data.Bases;

namespace Ogma3.Data.Faqs;

public class Faq : BaseModel
{
    public string Question { get; set; }
    public string Answer { get; set; }
    public string AnswerRendered { get; set; }
}