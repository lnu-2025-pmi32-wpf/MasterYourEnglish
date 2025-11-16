namespace MasterYourEnglish.BLL.DTOs
{
    using System.Collections.Generic;

    public class CreateBundleDto
    {
        // 1. КОНСТРУКТОР
        public CreateBundleDto()
        {
            this.NewFlashcards = new List<CreateFlashcardDto>();
        }

        // 2. ВЛАСТИВОСТІ
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int TopicId { get; set; }

        public string DifficultyLevel { get; set; } = "B2";

        public List<CreateFlashcardDto> NewFlashcards { get; set; }
    }
}