namespace MasterYourEnglish.BLL.DTOs
{
    using System.Collections.Generic;

    public class CreateTestDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int TopicId { get; set; }

        public string DifficultyLevel { get; set; } = "B2";

        public List<CreateQuestionDto> NewQuestions { get; set; } = new List<CreateQuestionDto>();
    }

    public class CreateQuestionDto
    {
        public string Text { get; set; } = string.Empty;

        public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();

        public int CorrectOptionIndex { get; set; }
    }

    public class CreateOptionDto
    {
        public string Text { get; set; } = string.Empty;
    }
}