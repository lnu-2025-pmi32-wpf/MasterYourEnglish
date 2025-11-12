namespace MasterYourEnglish.BLL.DTOs
{
    public class FlashcardSessionDto
    {
        public int FlashcardId { get; set; }
        public string Word { get; set; } = "";
        public string Transcription { get; set; } = "";
        public string Definition { get; set; } = "";
        public string Example { get; set; } = "";
        public string PartOfSpeech { get; set; } = "";
    }
}