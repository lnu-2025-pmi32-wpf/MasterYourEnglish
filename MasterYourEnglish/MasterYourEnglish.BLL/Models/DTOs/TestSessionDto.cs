using System.Collections.Generic;

namespace MasterYourEnglish.BLL.DTOs
{
    public class TestSessionDto
    {
        public int TestId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public List<TestQuestionDto> Questions { get; set; } = new List<TestQuestionDto>();
    }

    public class TestQuestionDto
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = "";
        public List<TestOptionDto> Options { get; set; } = new List<TestOptionDto>();
    }

    public class TestOptionDto
    {
        public int OptionId { get; set; }
        public string Text { get; set; } = "";
    }
}