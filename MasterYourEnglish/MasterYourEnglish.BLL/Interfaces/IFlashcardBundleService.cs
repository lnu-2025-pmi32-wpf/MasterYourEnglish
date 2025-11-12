using MasterYourEnglish.BLL.Models.DTOs;

namespace MasterYourEnglish.BLL.Interfaces
{
    public interface IFlashcardBundleService
    {
        Task<IEnumerable<FlashcardBundleCardDto>> GetPublishedBundlesAsync(string sortBy, bool ascending);
    }
}
