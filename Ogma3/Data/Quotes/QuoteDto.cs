using AutoMapper;

namespace Ogma3.Data.Quotes
{
    public class QuoteDto
    {
        public string Body { get; init; }
        public string Author { get; init; }

        public class Mapping : Profile
        {
            public Mapping() => CreateMap<Quote, QuoteDto>();
        }
    }
}