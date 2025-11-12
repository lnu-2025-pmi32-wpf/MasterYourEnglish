using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Models.DTOs
{
    public class SavedFlashcardDto
    {
        public int FlashcardId { get; set; }
        public string Word { get; set; } = "";
        public string PartOfSpeech { get; set; } = "";
        public string Difficulty { get; set; } = "";
        public string Transcription { get; set; } = "";
        public string Meaning { get; set; } = "";
        public string Example { get; set; } = "";
        public string CategoryName { get; set; } = "";
    }
}
