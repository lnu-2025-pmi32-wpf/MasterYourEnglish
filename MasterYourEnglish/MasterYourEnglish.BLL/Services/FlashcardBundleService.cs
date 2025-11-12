using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Services
{
    public class FlashcardBundleService : IFlashcardBundleService
    {
        private readonly IFlashcardBundleRepository _bundleRepository;

        public FlashcardBundleService(IFlashcardBundleRepository bundleRepository)
        {
            _bundleRepository = bundleRepository;
        }

        public async Task<IEnumerable<FlashcardBundleCardDto>> GetPublishedBundlesAsync(string searchTerm, string sortBy, bool ascending)
        {
            var bundles = await _bundleRepository.GetPublishedBundlesWithDetailsAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerSearch = searchTerm.ToLower();
                bundles = bundles.Where(b => b.Title.ToLower().Contains(lowerSearch) ||
                                             (b.Topic != null && b.Topic.Name.ToLower().Contains(lowerSearch)));
            }

            switch (sortBy)
            {
                case "Level":
                    bundles = ascending
                        ? bundles.OrderBy(b => b.DifficultyLevel)
                        : bundles.OrderByDescending(b => b.DifficultyLevel);
                    break;
                case "Name":
                default:
                    bundles = ascending
                        ? bundles.OrderBy(b => b.Title)
                        : bundles.OrderByDescending(b => b.Title);
                    break;
            }

            return bundles.Select(b => new FlashcardBundleCardDto
            {
                BundleId = b.FlashcardsBundleId,
                Description = b.Description,
                BundleName = b.Title,
                CategoryName = b.Topic?.Name ?? "General", 
                DifficultyLevel = b.DifficultyLevel ?? "N/A"
            });
        }
    }
}