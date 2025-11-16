namespace MasterYourEnglish.BLL.DTOs
{
    public class FlashcardSessionDto
    {
        public int FlashcardId { get; set; }

        public string Word { get; set; } = string.Empty;

        public string Transcription { get; set; } = string.Empty;

        public string Definition { get; set; } = string.Empty;

        public string Example { get; set; } = string.Empty;

        public string PartOfSpeech { get; set; } = string.Empty;
    }
}