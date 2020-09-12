namespace Ogma3.Data.Models
{
    public class ClubStory
    {
        public Club Club { get; set; }
        public long ClubId { get; set; }

        public Story Story { get; set; }
        public long StoryId { get; set; }
    }
}