namespace MasterYourEnglish.DAL.Entities
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string Text { get; set; }

        public string QuestionType { get; set; }

        public string? DifficultyLevel { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public User? CreatedByUser { get; set; }

        public int? TopicId { get; set; }

        public Topic? Topic { get; set; }

        public ICollection<QuestionOption>? QuestionOptions { get; set; }

        public ICollection<TestQuestion>? TestQuestions { get; set; }

        public ICollection<TestAttemptAnswer>? TestAttemptsAnswers { get; set; }
    }
}
