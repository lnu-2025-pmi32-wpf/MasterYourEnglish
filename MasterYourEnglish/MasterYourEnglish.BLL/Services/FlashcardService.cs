namespace MasterYourEnglish.BLL.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class FlashcardService : IFlashcardService
    {
        private readonly IFlashcardRepository flashcardRepository;
        private readonly IRepository<SavedFlashcard> savedFlashcardRepository;

        public FlashcardService(
            IFlashcardRepository flashcardRepository,
            IRepository<SavedFlashcard> savedFlashcardRepository)
        {
            this.flashcardRepository = flashcardRepository;
            this.savedFlashcardRepository = savedFlashcardRepository;
        }

        public async Task<IEnumerable<SavedFlashcardDto>> GetSavedFlashcardsAsync(int userId, string searchTerm, string sortBy, bool ascending)
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

        public async Task<bool> RemoveFromSavedAsync(int userId, int flashcardId)
        {
            var savedEntry = await this.savedFlashcardRepository.FindAsync(
                sf => sf.UserId == userId && sf.FlashcardId == flashcardId);

            if (savedEntry.FirstOrDefault() != null)
            {
                this.savedFlashcardRepository.DeleteAsync(savedEntry.First());
                return true;
            }

            return false;
        }

        public async Task<bool> AddToSavedAsync(int userId, int flashcardId)
        {
            var existing = await this.savedFlashcardRepository.FindAsync(
                sf => sf.UserId == userId && sf.FlashcardId == flashcardId);

            if (existing.Any())
            {
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

        public async Task<List<FlashcardSessionDto>> GetSavedFlashcardsForSessionAsync(int userId)
        {
            var flashcards = await this.flashcardRepository.GetSavedFlashcardsForUserAsync(userId);

            if (flashcards == null)
            {
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
    }
}