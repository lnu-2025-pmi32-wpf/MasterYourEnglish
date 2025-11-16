namespace MasterYourEnglish.BLL.DTOs
{
    public class CreateFlashcardDto
    {
        public string Word { get; set; } = string.Empty;

        public string Meaning { get; set; } = string.Empty;

        public string PartOfSpeech { get; set; } = string.Empty;

        public string Example { get; set; } = string.Empty;

        public string Transcription { get; set; } = string.Empty;

        public string DifficultyLevel { get; set; } = "B2";
    }
}