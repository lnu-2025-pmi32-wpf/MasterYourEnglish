using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using MasterYourEnglish.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
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
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly IRepository<FlashcardBundleItem> _bundleItemRepository;

        public FlashcardBundleService(
            IFlashcardBundleRepository bundleRepository,
            IFlashcardsBundleAttemptRepository attemptRepository,
            IRepository<FlashcardAttemptAnswer> answerRepository,
            IFlashcardRepository flashcardRepository,
            IRepository<FlashcardBundleItem> bundleItemRepository)
        {
            _bundleRepository = bundleRepository;
            _attemptRepository = attemptRepository;
            _answerRepository = answerRepository;
            _flashcardRepository = flashcardRepository;
            _bundleItemRepository = bundleItemRepository;
        }

        public async Task<IEnumerable<FlashcardBundleCardDto>> GetPublishedBundlesAsync(string searchTerm, string sortBy, bool ascending)
        {
            var bundles = await _bundleRepository.GetPublishedBundlesWithDetailsAsync();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerSearch = searchTerm.ToLower();
                bundles = bundles.Where(b => b.Title.ToLower().Contains(lowerSearch) ||
                                             (b.Topic?.Name ?? "").ToLower().Contains(lowerSearch) ||
                                             (b.Description ?? "").ToLower().Contains(lowerSearch));
            }
            switch (sortBy)
            {
                case "Level":
                    bundles = ascending ? bundles.OrderBy(b => b.DifficultyLevel) : bundles.OrderByDescending(b => b.DifficultyLevel);
                    break;
                case "Name":
                default:
                    bundles = ascending ? bundles.OrderBy(b => b.Title) : bundles.OrderByDescending(b => b.Title);
                    break;
            }
            return bundles.Select(b => new FlashcardBundleCardDto
            {
                BundleId = b.FlashcardsBundleId,
                BundleName = b.Title,
                CategoryName = b.Topic?.Name ?? "General",
                DifficultyLevel = b.DifficultyLevel ?? "N/A",
                Description = b.Description ?? ""
            });
        }

        public async Task<List<FlashcardSessionDto>> GetFlashcardSessionAsync(int bundleId)
        {
            var bundle = await _bundleRepository.GetFlashcardBundleWithDetailsAsync(bundleId);
            if (bundle == null || bundle.FlashcardsBundleItems == null) { return new List<FlashcardSessionDto>(); }
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
            foreach (var answer in answers) { await _answerRepository.AddAsync(answer); }
        }
        public async Task<List<FlashcardSessionDto>> GetGeneratedSessionAsync(int userId, List<string> levels, Dictionary<int, int> topicRequests)
        {
            var allFoundFlashcards = new List<Flashcard>();

            foreach (var request in topicRequests)
            {
                int topicId = request.Key;
                int count = request.Value;
                if (count > 0)
                {
                    var cards = await _flashcardRepository.GetFlashcardsByCriteriaAsync(topicId, levels, count);
                    allFoundFlashcards.AddRange(cards);
                }
            }

            allFoundFlashcards = allFoundFlashcards.Distinct().ToList();

            return allFoundFlashcards.Select(f => new FlashcardSessionDto
            {
                FlashcardId = f.FlashcardId,
                Word = f.Word,
                Transcription = f.Transcription ?? "",
                Definition = f.Meaning ?? "",
                Example = f.Example ?? "",
                PartOfSpeech = f.PartOfSpeech ?? ""
            }).ToList();
        }

        public async Task<int> CreateNewBundleAsync(CreateBundleDto bundleDto, int userId)
        {
            // DANGER: This is not transactional. See warning.

            var newBundle = new FlashcardBundle
            {
                Title = bundleDto.Title,
                Description = bundleDto.Description,
                TopicId = bundleDto.TopicId,
                DifficultyLevel = bundleDto.DifficultyLevel,
                CreatedAt = DateTime.UtcNow,
                IsPublished = true, // User-created bundles are private
                IsUserCreated = true,
                CreatedBy = userId,
                TotalFlashcardsCount = bundleDto.NewFlashcards.Count
            };

            await _bundleRepository.AddAsync(newBundle);
            int position = 1;
            foreach (var cardDto in bundleDto.NewFlashcards)
            {
                var newCard = new Flashcard
                {
                    Word = cardDto.Word,
                    Meaning = cardDto.Meaning,
                    Example = cardDto.Example,
                    Transcription = cardDto.Transcription,
                    PartOfSpeech = cardDto.PartOfSpeech,
                    DifficultyLevel = cardDto.DifficultyLevel,
                    TopicId = bundleDto.TopicId,
                    CreatedAt = DateTime.UtcNow,
                    IsUserCreated = true,
                    CreatedBy = userId
                };
                await _flashcardRepository.AddAsync(newCard);
                var newItem = new FlashcardBundleItem
                {
                    FlashcardsBundleId = newBundle.FlashcardsBundleId,
                    FlashcardId = newCard.FlashcardId,
                    Position = position++
                };
                await _bundleItemRepository.AddAsync(newItem);
            }

            return newBundle.FlashcardsBundleId;
        }
    }
}