namespace MasterYourEnglish.DAL.Entities
{
    public class Test
    {
        public int TestId { get; set; }
        public string Title { get; set; } 
        public string? Description { get; set; }
        public string? DifficultyLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublished { get; set; }
        public bool IsUserCreated { get; set; }
        public int TotalQuestionsCount { get; set; }

        public int? TopicId { get; set; }
        public Topic? Topic { get; set; }

        public int? CreatedBy { get; set; }
        public User? CreatedByUser { get; set; }

        public ICollection<TestQuestion>? TestQuestions { get; set; }
        public ICollection<TestAttempt>? TestAttempts { get; set; }
    }
}
