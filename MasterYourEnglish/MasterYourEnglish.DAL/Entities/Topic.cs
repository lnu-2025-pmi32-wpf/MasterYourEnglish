namespace MasterYourEnglish.DAL.Entities
{
    public class Topic
    {
        public int TopicId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Flashcard>? Flashcards { get; set; }
        public ICollection<FlashcardBundle>? FlashcardsBundles { get; set; }
        public ICollection<Test>? Tests { get; set; }
        public ICollection<Question>? Questions { get; set; }
    }
}
