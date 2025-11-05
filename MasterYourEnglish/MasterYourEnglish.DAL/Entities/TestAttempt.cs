namespace MasterYourEnglish.DAL.Entities
{
    public class TestAttempt
    {
        public int AttemptId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public float Score { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; } 

        public int UserId { get; set; }
        public User User { get; set; } 

        public ICollection<TestAttemptAnswer>? TestAttemptsAnswers { get; set; }
    }
}
