namespace MasterYourEnglish.BLL.DTOs
{
    public class TestGenerationRequest
    {
        public int TopicId { get; set; }

        public int SingleChoiceCount { get; set; }

        public int MultipleChoiceCount { get; set; }

        public int MatchingCount { get; set; }

        public int TotalCount => this.SingleChoiceCount + this.MultipleChoiceCount + this.MatchingCount;
    }
}
