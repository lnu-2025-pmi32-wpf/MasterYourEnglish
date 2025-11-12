namespace MasterYourEnglish.BLL.DTOs
{
    public class CreateFlashcardDto
    {
        public string Word { get; set; } = "";
        public string Meaning { get; set; } = "";
        public string PartOfSpeech { get; set; } = "";
        public string Example { get; set; } = "";
        public string Transcription { get; set; } = "";
        public string DifficultyLevel { get; set; } = "B2";
    }
}