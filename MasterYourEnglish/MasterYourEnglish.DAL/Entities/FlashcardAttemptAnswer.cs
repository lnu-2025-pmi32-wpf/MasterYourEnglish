namespace MasterYourEnglish.DAL.Entities
{
    public class FlashcardAttemptAnswer
    {
        public int AnswerId { get; set; }

        public bool IsKnown { get; set; }

        public int AttemptId { get; set; }

        public FlashcardBundleAttempt Attempt { get; set; }

        public int FlashcardId { get; set; }

        public Flashcard Flashcard { get; set; }
    }
}
