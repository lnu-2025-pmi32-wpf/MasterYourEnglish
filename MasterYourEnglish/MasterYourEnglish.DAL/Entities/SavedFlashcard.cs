namespace MasterYourEnglish.DAL.Entities
{
    public class SavedFlashcard
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int FlashcardId { get; set; }

        public Flashcard Flashcard { get; set; }
    }
}
