using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Interfaces;

namespace MasterYourEnglish.BLL.Services
{
    public class FlashcardBundleService : IFlashcardBundleService
    {
        private readonly IFlashcardBundleRepository _bundleRepository;

        public FlashcardBundleService(IFlashcardBundleRepository bundleRepository)
        {
            _bundleRepository = bundleRepository;
        }

        public async Task<IEnumerable<FlashcardBundleCardDto>> GetPublishedBundlesAsync(string sortBy, bool ascending)
        {
            var bundles = await _bundleRepository.GetAllAsync();

            var sortedBundles = bundles;
            switch (sortBy)
            {
                case "Level":
                    sortedBundles = ascending
                        ? bundles.OrderBy(b => b.DifficultyLevel)
                        : bundles.OrderByDescending(b => b.DifficultyLevel);
                    break;
                case "Name":
                default:
                    sortedBundles = ascending
                        ? bundles.OrderBy(b => b.Title)
                        : bundles.OrderByDescending(b => b.Title);
                    break;
            }

            return sortedBundles.Select(b => new FlashcardBundleCardDto
            {
                BundleId = b.FlashcardsBundleId,
                BundleName = b.Title,
                CategoryName = b.Topic?.Name ?? "General"
            });
        }
    }
}
