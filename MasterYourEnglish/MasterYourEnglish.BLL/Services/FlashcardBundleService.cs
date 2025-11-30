namespace MasterYourEnglish.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using MasterYourEnglish.DAL.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class FlashcardBundleService : IFlashcardBundleService
    {
        private readonly IFlashcardBundleRepository bundleRepository;
        private readonly IFlashcardsBundleAttemptRepository attemptRepository;
        private readonly IRepository<FlashcardAttemptAnswer> answerRepository;
        private readonly IFlashcardRepository flashcardRepository;
        private readonly IRepository<FlashcardBundleItem> bundleItemRepository;
        private readonly ILogger<FlashcardBundleService> logger;

        public FlashcardBundleService(
            IFlashcardBundleRepository bundleRepository,
            IFlashcardsBundleAttemptRepository attemptRepository,
            IRepository<FlashcardAttemptAnswer> answerRepository,
            IFlashcardRepository flashcardRepository,
            IRepository<FlashcardBundleItem> bundleItemRepository,
            ILogger<FlashcardBundleService> logger)
        {
            this.bundleRepository = bundleRepository;
            this.attemptRepository = attemptRepository;
            this.answerRepository = answerRepository;
            this.flashcardRepository = flashcardRepository;
            this.bundleItemRepository = bundleItemRepository;
            this.logger = logger;
        }

        public async Task<IEnumerable<FlashcardBundleCardDto>> GetPublishedBundlesAsync(string searchTerm, string sortBy, bool ascending)
        {
            this.logger.LogInformation("Getting published bundles. Search: '{SearchTerm}', Sort: {SortBy}", searchTerm, sortBy);
            try
            {
                var bundles = await this.bundleRepository.GetPublishedBundlesWithDetailsAsync();
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string lowerSearch = searchTerm.ToLower();
                    bundles = bundles.Where(b => b.Title.ToLower().Contains(lowerSearch) ||
                                                 (b.Topic?.Name ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                                 (b.Description ?? string.Empty).ToLower().Contains(lowerSearch));
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
                    Description = b.Description ?? string.Empty,
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting published bundles.");
                throw;
            }
        }

        public async Task<List<FlashcardSessionDto>> GetFlashcardSessionAsync(int bundleId)
        {
            this.logger.LogInformation("Getting session for bundle ID: {BundleId}", bundleId);
            try
            {
                var bundle = await this.bundleRepository.GetFlashcardBundleWithDetailsAsync(bundleId);
                if (bundle == null || bundle.FlashcardsBundleItems == null)
                {
                    this.logger.LogWarning("Bundle ID {BundleId} not found or empty.", bundleId);
                    return new List<FlashcardSessionDto>();
                }

                return bundle.FlashcardsBundleItems
                    .OrderBy(item => item.Position)
                    .Select(item => new FlashcardSessionDto
                    {
                        FlashcardId = item.Flashcard.FlashcardId,
                        Word = item.Flashcard.Word,
                        Transcription = item.Flashcard.Transcription ?? string.Empty,
                        Definition = item.Flashcard.Meaning ?? string.Empty,
                        Example = item.Flashcard.Example ?? string.Empty,
                        PartOfSpeech = item.Flashcard.PartOfSpeech ?? string.Empty,
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting session for bundle ID {BundleId}", bundleId);
                throw;
            }
        }

        public async Task SaveSessionAttemptAsync(int bundleId, int userId, Dictionary<int, bool> results)
        {
            this.logger.LogInformation("Saving session attempt for bundle {BundleId}, user {UserId}", bundleId, userId);
            try
            {
                var newAttempt = new FlashcardBundleAttempt
                {
                    FlashcardsBundleId = bundleId,
                    UserId = userId,
                    StartedAt = DateTime.UtcNow,
                    FinishedAt = DateTime.UtcNow,
                };
                await this.attemptRepository.AddAsync(newAttempt);
                var answers = results.Select(res => new FlashcardAttemptAnswer
                {
                    AttemptId = newAttempt.AttemptId,
                    FlashcardId = res.Key,
                    IsKnown = res.Value,
                }).ToList();
                foreach (var answer in answers)
                {
                    await this.answerRepository.AddAsync(answer);
                }

                this.logger.LogInformation("Session attempt saved successfully. AttemptID: {AttemptId}", newAttempt.AttemptId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error saving session attempt for bundle {BundleId}, user {UserId}", bundleId, userId);
                throw;
            }
        }

        public async Task<List<FlashcardSessionDto>> GetGeneratedSessionAsync(int userId, List<string> levels, Dictionary<int, int> topicRequests)
        {
            this.logger.LogInformation("Generating flashcard session for user {UserId}", userId);
            try
            {
                var allFoundFlashcards = new List<Flashcard>();

                foreach (var request in topicRequests)
                {
                    int topicId = request.Key;
                    int count = request.Value;
                    if (count > 0)
                    {
                        var cards = await this.flashcardRepository.GetFlashcardsByCriteriaAsync(topicId, levels, count);
                        allFoundFlashcards.AddRange(cards);
                    }
                }

                allFoundFlashcards = allFoundFlashcards.Distinct().ToList();

                this.logger.LogInformation("Generated {Count} flashcards.", allFoundFlashcards.Count);

                return allFoundFlashcards.Select(f => new FlashcardSessionDto
                {
                    FlashcardId = f.FlashcardId,
                    Word = f.Word,
                    Transcription = f.Transcription ?? string.Empty,
                    Definition = f.Meaning ?? string.Empty,
                    Example = f.Example ?? string.Empty,
                    PartOfSpeech = f.PartOfSpeech ?? string.Empty,
                }).ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error generating flashcard session for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> CreateNewBundleAsync(CreateBundleDto bundleDto, int userId)
        {
            this.logger.LogInformation("Creating new bundle '{Title}' by user {UserId}", bundleDto.Title, userId);
            try
            {
                var newBundle = new FlashcardBundle
                {
                    Title = bundleDto.Title,
                    Description = bundleDto.Description,
                    TopicId = bundleDto.TopicId,
                    DifficultyLevel = bundleDto.DifficultyLevel,
                    CreatedAt = DateTime.UtcNow,
                    IsPublished = true,
                    IsUserCreated = true,
                    CreatedBy = userId,
                    TotalFlashcardsCount = bundleDto.NewFlashcards.Count,
                };

                await this.bundleRepository.AddAsync(newBundle);
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
                        CreatedBy = userId,
                    };
                    await this.flashcardRepository.AddAsync(newCard);
                    var newItem = new FlashcardBundleItem
                    {
                        FlashcardsBundleId = newBundle.FlashcardsBundleId,
                        FlashcardId = newCard.FlashcardId,
                        Position = position++,
                    };
                    await this.bundleItemRepository.AddAsync(newItem);
                }

                this.logger.LogInformation("Bundle created successfully. ID: {BundleId}", newBundle.FlashcardsBundleId);
                return newBundle.FlashcardsBundleId;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating bundle by user {UserId}", userId);
                throw;
            }
        }
    }
}