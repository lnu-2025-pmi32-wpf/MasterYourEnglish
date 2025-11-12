using MasterYourEnglish.BLL.Models.DTOs;

namespace MasterYourEnglish.BLL.Interfaces
{
    public interface IFlashcardBundleService
    {
        Task<IEnumerable<FlashcardBundleCardDto>> GetPublishedBundlesAsync(string searchTerm, string sortBy, bool ascending);
    }
}
