using MasterYourEnglish.BLL.Models.DTOs;

namespace MasterYourEnglish.BLL.Interfaces
{
    public interface ITestService
    {
        Task<IEnumerable<TestCardDto>> GetPublishedTestsForDashboardAsync();
        Task<IEnumerable<TestCardDto>> GetPublishedTestsByTopicAsync(int topicId);
        Task<IEnumerable<TestCardDto>> GetPublishedTestsAsync(string searchTerm, string sortBy, bool ascending);
    }
}