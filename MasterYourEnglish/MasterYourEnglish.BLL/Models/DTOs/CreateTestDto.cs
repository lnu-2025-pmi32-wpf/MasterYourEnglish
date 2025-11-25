using System.Collections.Generic;

namespace MasterYourEnglish.BLL.DTOs
{
    public class CreateTestDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int TopicId { get; set; }
        public string DifficultyLevel { get; set; } = "B2";
        public List<CreateQuestionDto> NewQuestions { get; set; } = new List<CreateQuestionDto>();
    }

    public class CreateQuestionDto
    {
        public string Text { get; set; } = "";
        public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();
        public int CorrectOptionIndex { get; set; } 
    }

    public class CreateOptionDto
    {
        public string Text { get; set; } = "";
    }
}