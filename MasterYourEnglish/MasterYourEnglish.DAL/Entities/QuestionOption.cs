namespace MasterYourEnglish.DAL.Entities
{
    public class QuestionOption
    {
        public int OptionId { get; set; }

        public string Text { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }

        public Question Question { get; set; }

        public ICollection<TestAttemptAnswerSelectedOption>? SelectedInAnswers { get; set; }
    }
}
