namespace Ogma3.Data.Models
{
    public class Faq : BaseModel
    {
        public string Question { get; init; }
        public string Answer { get; init; }
        public string AnswerRendered { get; init; }
    }
}