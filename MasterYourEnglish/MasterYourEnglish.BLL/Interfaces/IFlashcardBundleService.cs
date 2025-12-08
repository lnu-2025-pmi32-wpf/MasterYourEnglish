namespace MasterYourEnglish.BLL.Interfaces
{
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Models.DTOs;

    public interface IFlashcardBundleService
    {
        Task<IEnumerable<FlashcardBundleCardDto>> GetPublishedBundlesAsync(string searchTerm, string sortBy, bool ascending);

        Task<List<FlashcardSessionDto>> GetFlashcardSessionAsync(int bundleId);

        Task SaveSessionAttemptAsync(int bundleId, int userId, Dictionary<int, bool> results);

        Task<List<FlashcardSessionDto>> GetGeneratedSessionAsync(int userId, List<string> levels, Dictionary<int, int> topicRequests);

        Task<int> CreateNewBundleAsync(CreateBundleDto bundleDto, int userId);

        Task<FlashcardBundleCardDto> GetBundleByIdAsync(int bundleId);
    }
}
