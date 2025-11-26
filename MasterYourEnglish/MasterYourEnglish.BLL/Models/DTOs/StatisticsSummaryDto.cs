namespace MasterYourEnglish.BLL.Models.DTOs
{
    public class StatisticsSummaryDto
    {
        public int TestsTakenLast2Weeks { get; set; }

        public int FlashcardSessionsLast2Weeks { get; set; }

        public float? AverageScore { get; set; }

        public int TotalAttempts { get; set; }
    }
}
