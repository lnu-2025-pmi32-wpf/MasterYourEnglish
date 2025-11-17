namespace MasterYourEnglish.DAL.Entities
{
    public class FlashcardBundle
    {
        public int FlashcardsBundleId { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? DifficultyLevel { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsPublished { get; set; }

        public bool IsUserCreated { get; set; }

        public int TotalFlashcardsCount { get; set; }

        public int? TopicId { get; set; }

        public Topic? Topic { get; set; }

        public int? CreatedBy { get; set; }

        public User? CreatedByUser { get; set; }

        public ICollection<FlashcardBundleItem>? FlashcardsBundleItems { get; set; }

        public ICollection<FlashcardBundleAttempt>? FlashcardsBundleAttempts { get; set; }
    }
}
