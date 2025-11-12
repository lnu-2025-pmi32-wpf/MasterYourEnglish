using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using MasterYourEnglish.DAL.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Services
{
    public class FlashcardBundleService : IFlashcardBundleService
    {
        private readonly IFlashcardBundleRepository _bundleRepository;
        private readonly IFlashcardsBundleAttemptRepository _attemptRepository;
        private readonly IRepository<FlashcardAttemptAnswer> _answerRepository;

        public FlashcardBundleService(IFlashcardBundleRepository bundleRepository,
            IFlashcardsBundleAttemptRepository attemptRepository,
            IRepository<FlashcardAttemptAnswer> answerRepository)
        {
            _bundleRepository = bundleRepository;
            _attemptRepository = attemptRepository;
            _answerRepository = answerRepository;
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

        public async Task<List<FlashcardSessionDto>> GetFlashcardSessionAsync(int bundleId)
        {
            var bundle = await _bundleRepository.GetFlashcardBundleWithDetailsAsync(bundleId);
            if (bundle == null || bundle.FlashcardsBundleItems == null)
            {
                return new List<FlashcardSessionDto>(); 
            }

            return bundle.FlashcardsBundleItems
                .OrderBy(item => item.Position) 
                .Select(item => new FlashcardSessionDto
                {
                    FlashcardId = item.Flashcard.FlashcardId,
                    Word = item.Flashcard.Word,
                    Transcription = item.Flashcard.Transcription ?? "",
                    Definition = item.Flashcard.Meaning ?? "",
                    Example = item.Flashcard.Example ?? "",
                    PartOfSpeech = item.Flashcard.PartOfSpeech ?? ""
                })
                .ToList();
        }
        public async Task SaveSessionAttemptAsync(int bundleId, int userId, Dictionary<int, bool> results)
        {
            var newAttempt = new FlashcardBundleAttempt
            {
                FlashcardsBundleId = bundleId,
                UserId = userId,
                StartedAt = DateTime.UtcNow, 
                FinishedAt = DateTime.UtcNow
            };

            await _attemptRepository.AddAsync(newAttempt);

            var answers = results.Select(res => new FlashcardAttemptAnswer
            {
                AttemptId = newAttempt.AttemptId,
                FlashcardId = res.Key, 
                IsKnown = res.Value   
            }).ToList();

            foreach (var answer in answers)
            {
                await _answerRepository.AddAsync(answer);
            }
        }
    }
}