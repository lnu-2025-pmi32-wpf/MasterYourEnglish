namespace MasterYourEnglish.DAL.Entities
{
    public class FlashcardBundleAttempt
    {
        public int AttemptId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public int FlashcardsBundleId { get; set; }
        public FlashcardBundle FlashcardsBundle { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<FlashcardAttemptAnswer>? FlashcardsAttemptAnswers { get; set; }
    }
}
