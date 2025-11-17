namespace MasterYourEnglish.BLL.Models.DTOs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SavedFlashcardDto
    {
        public int FlashcardId { get; set; }

        public string Word { get; set; } = string.Empty;

        public string PartOfSpeech { get; set; } = string.Empty;

        public string Difficulty { get; set; } = string.Empty;

        public string Transcription { get; set; } = string.Empty;

        public string Meaning { get; set; } = string.Empty;

        public string Example { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;
    }
}
