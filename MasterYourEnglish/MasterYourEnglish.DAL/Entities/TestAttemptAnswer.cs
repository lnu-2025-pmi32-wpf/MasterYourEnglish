namespace MasterYourEnglish.DAL.Entities
{
    public class TestAttemptAnswer
    {
        public int AttemptAnswersId { get; set; }

        public string? UserAnswerText { get; set; }

        public bool IsCorrect { get; set; }

        public int AttemptId { get; set; }

        public TestAttempt Attempt { get; set; }

        public int QuestionId { get; set; }

        public Question Question { get; set; }

        public int? SelectedOptionId { get; set; }

        public QuestionOption? SelectedOption { get; set; }

        public ICollection<TestAttemptAnswerSelectedOption>? SelectedOptions { get; set; }
    }
}
