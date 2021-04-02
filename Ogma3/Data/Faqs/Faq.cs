using Ogma3.Data.Bases;

namespace Ogma3.Data.Faqs
{
    public class Faq : BaseModel
    {
        public string Question { get; init; }
        public string Answer { get; init; }
        public string AnswerRendered { get; init; }
    }
}