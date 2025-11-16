namespace MasterYourEnglish.DAL.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Role { get; set; } = "user";

        public ICollection<Flashcard>? Flashcards { get; set; }

        public ICollection<FlashcardBundle>? FlashcardsBundles { get; set; }

        public ICollection<SavedFlashcard>? SavedFlashcards { get; set; }

        public ICollection<FlashcardBundleAttempt>? FlashcardsBundleAttempts { get; set; }

        public ICollection<Test>? Tests { get; set; }

        public ICollection<Question>? Questions { get; set; }

        public ICollection<TestAttempt>? TestAttempts { get; set; }
    }
}
