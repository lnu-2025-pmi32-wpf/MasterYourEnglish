using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Models.DTOs;

namespace MasterYourEnglish.BLL.Interfaces
{
    public interface IFlashcardBundleService
    {
        Task<IEnumerable<FlashcardBundleCardDto>> GetPublishedBundlesAsync(string searchTerm, string sortBy, bool ascending);
        Task<List<FlashcardSessionDto>> GetFlashcardSessionAsync(int bundleId);
        Task SaveSessionAttemptAsync(int bundleId, int userId, Dictionary<int, bool> results);
    }
}
