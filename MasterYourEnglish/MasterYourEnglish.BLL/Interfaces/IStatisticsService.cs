namespace MasterYourEnglish.BLL.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.Models.DTOs;

    public interface IStatisticsService
    {
        Task<IEnumerable<AttemptHistoryDto>> GetUserAttemptHistoryAsync(int userId);

        Task<StatisticsSummaryDto> GetUserStatisticsSummaryAsync(int userId);
    }
}
