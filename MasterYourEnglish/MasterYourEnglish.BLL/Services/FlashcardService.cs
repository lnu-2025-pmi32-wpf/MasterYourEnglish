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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class FlashcardService : IFlashcardService
    {
        private readonly IFlashcardRepository flashcardRepository;
        private readonly IRepository<SavedFlashcard> savedFlashcardRepository;
        private readonly ILogger<FlashcardService> logger;

        public FlashcardService(
            IFlashcardRepository flashcardRepository,
            IRepository<SavedFlashcard> savedFlashcardRepository,
            ILogger<FlashcardService> logger)
        {
            this.flashcardRepository = flashcardRepository;
            this.savedFlashcardRepository = savedFlashcardRepository;
            this.logger = logger;
        }

        public async Task<IEnumerable<SavedFlashcardDto>> GetSavedFlashcardsAsync(int userId, string searchTerm, string sortBy, bool ascending)
        {
            this.logger.LogInformation("Getting saved flashcards for user {UserId}", userId);
            try
            {
                var flashcards = await this.flashcardRepository.GetSavedFlashcardsForUserAsync(userId);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string lowerSearch = searchTerm.ToLower();
                    flashcards = flashcards.Where(f => f.Word.ToLower().Contains(lowerSearch) ||
                                                     (f.Meaning ?? string.Empty).ToLower().Contains(lowerSearch));
                }

                switch (sortBy)
                {
                    case "Level":
                        flashcards = ascending
                            ? flashcards.OrderBy(f => f.DifficultyLevel)
                            : flashcards.OrderByDescending(f => f.DifficultyLevel);
                        break;
                    case "Name":
                    default:
                        flashcards = ascending
                            ? flashcards.OrderBy(f => f.Word)
                            : flashcards.OrderByDescending(f => f.Word);
                        break;
                }

                return flashcards.Select(f => new SavedFlashcardDto
                {
                    FlashcardId = f.FlashcardId,
                    Word = f.Word,
                    Difficulty = f.DifficultyLevel ?? "N/A",
                    PartOfSpeech = f.PartOfSpeech ?? string.Empty,
                    Transcription = f.Transcription ?? string.Empty,
                    Meaning = f.Meaning ?? string.Empty,
                    Example = f.Example ?? string.Empty,
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting saved flashcards for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> RemoveFromSavedAsync(int userId, int flashcardId)
        {
            this.logger.LogInformation("Removing flashcard {FlashcardId} from saved for user {UserId}", flashcardId, userId);
            try
            {
                var savedEntry = await this.savedFlashcardRepository.FindAsync(
                    sf => sf.UserId == userId && sf.FlashcardId == flashcardId);

                if (savedEntry.FirstOrDefault() != null)
                {
                    this.savedFlashcardRepository.DeleteAsync(savedEntry.First());
                    return true;
                }

                this.logger.LogWarning("Flashcard {FlashcardId} not found in saved for user {UserId}", flashcardId, userId);
                return false;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error removing flashcard {FlashcardId} for user {UserId}", flashcardId, userId);
                throw;
            }
        }

        public async Task<bool> AddToSavedAsync(int userId, int flashcardId)
        {
            this.logger.LogInformation("Adding flashcard {FlashcardId} to saved for user {UserId}", flashcardId, userId);
            try
            {
                var existing = await this.savedFlashcardRepository.FindAsync(
                    sf => sf.UserId == userId && sf.FlashcardId == flashcardId);

                if (existing.Any())
                {
                    this.logger.LogInformation("Flashcard {FlashcardId} already exists in saved for user {UserId}", flashcardId, userId);
                    return true;
                }

                var newSavedFlashcard = new SavedFlashcard
                {
                    UserId = userId,
                    FlashcardId = flashcardId,
                };

                await this.savedFlashcardRepository.AddAsync(newSavedFlashcard);
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error adding flashcard {FlashcardId} for user {UserId}", flashcardId, userId);
                throw;
            }
        }

        public async Task<List<FlashcardSessionDto>> GetSavedFlashcardsForSessionAsync(int userId)
        {
            this.logger.LogInformation("Getting saved flashcards session for user {UserId}", userId);
            try
            {
                var flashcards = await this.flashcardRepository.GetSavedFlashcardsForUserAsync(userId);

                if (flashcards == null)
                {
                    this.logger.LogWarning("No saved flashcards found for user {UserId}", userId);
                    return new List<FlashcardSessionDto>();
                }

                return flashcards.Select(f => new FlashcardSessionDto
                {
                    FlashcardId = f.FlashcardId,
                    Word = f.Word,
                    Transcription = f.Transcription ?? string.Empty,
                    Definition = f.Meaning ?? string.Empty,
                    Example = f.Example ?? string.Empty,
                    PartOfSpeech = f.PartOfSpeech ?? string.Empty,
                })
                .ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting saved flashcards session for user {UserId}", userId);
                throw;
            }
        }
    }
}