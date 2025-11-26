namespace MasterYourEnglish.BLL.Models.DTOs
{
    public enum AttemptType
    {
        Test,
        Flashcard
    }

    public class AttemptHistoryDto
    {
        public AttemptType AttemptType { get; set; }

        public int AttemptId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? TopicName { get; set; }

        public float? Score { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public string? DifficultyLevel { get; set; }
    }
}
