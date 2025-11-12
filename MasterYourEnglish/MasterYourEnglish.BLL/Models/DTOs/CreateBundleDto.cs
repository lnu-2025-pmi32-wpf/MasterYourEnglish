using System.Collections.Generic;

namespace MasterYourEnglish.BLL.DTOs
{
    public class CreateBundleDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int TopicId { get; set; }
        public string DifficultyLevel { get; set; } = "B2";
        public List<CreateFlashcardDto> NewFlashcards { get; set; }

        public CreateBundleDto()
        {
            NewFlashcards = new List<CreateFlashcardDto>();
        }
    }
}