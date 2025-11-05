namespace MasterYourEnglish.DAL.Entities
{
    public class FlashcardBundleItem
    {
        public int FlashcardsBundleId { get; set; }
        public FlashcardBundle FlashcardsBundle { get; set; }

        public int FlashcardId { get; set; }
        public Flashcard Flashcard { get; set; } = null!;

        public int Position { get; set; }
    }
}
