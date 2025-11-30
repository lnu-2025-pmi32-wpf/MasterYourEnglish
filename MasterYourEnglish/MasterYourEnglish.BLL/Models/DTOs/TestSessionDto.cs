namespace MasterYourEnglish.BLL.DTOs
{
    using System.Collections.Generic;

    public class TestSessionDto
    {
        public int TestId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<TestQuestionDto> Questions { get; set; } = new List<TestQuestionDto>();
    }

    public class TestQuestionDto
    {
        public int QuestionId { get; set; }

        public string Text { get; set; } = string.Empty;

        public List<TestOptionDto> Options { get; set; } = new List<TestOptionDto>();
    }

    public class TestOptionDto
    {
        public int OptionId { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}