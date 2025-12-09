namespace MasterYourEnglish.BLL.Interfaces
{
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Models.DTOs;

    public interface ITestService
    {
        Task<IEnumerable<TestCardDto>> GetPublishedTestsForDashboardAsync();

        Task<IEnumerable<TestCardDto>> GetPublishedTestsByTopicAsync(int topicId);

        Task<IEnumerable<TestCardDto>> GetPublishedTestsAsync(string searchTerm, string sortBy, bool ascending);

        Task<TestSessionDto> GetTestSessionAsync(int testId);

        Task<int> SubmitTestAttemptAsync(int testId, int userId, Dictionary<int, int> answers);

        Task<int> CreateNewTestAsync(CreateTestDto testDto, int userId);

        Task<List<TestSessionDto>> GetGeneratedTestSessionAsync(int userId, List<string> levels, List<TestGenerationRequest> requests);
    }
}