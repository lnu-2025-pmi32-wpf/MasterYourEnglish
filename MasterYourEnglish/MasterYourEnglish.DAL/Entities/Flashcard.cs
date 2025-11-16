namespace MasterYourEnglish.DAL.Entities
{
    public class Flashcard
    {
        public int FlashcardId { get; set; }

        public string Word { get; set; } = null!;

        public string? DifficultyLevel { get; set; }

        public string? Transcription { get; set; }

        public string? Meaning { get; set; }

        public string? PartOfSpeech { get; set; }

        public string? Example { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsUserCreated { get; set; }

        public int? TopicId { get; set; }

        public Topic? Topic { get; set; }

        public int? CreatedBy { get; set; }

        public User? CreatedByUser { get; set; }

        public ICollection<FlashcardBundleItem>? FlashcardsBundleItems { get; set; }

        public ICollection<SavedFlashcard>? SavedByUsers { get; set; }

        public ICollection<FlashcardAttemptAnswer>? FlashcardsAttemptAnswers { get; set; }
    }
}
