namespace MasterYourEnglish.DAL.Entities
{
    public class TestAttemptAnswerSelectedOption
    {
        public int AttemptAnswersId { get; set; }
        public TestAttemptAnswer AttemptAnswer { get; set; }

        public int SelectedOptionId { get; set; }
        public QuestionOption SelectedOption { get; set; } 
    }
}
