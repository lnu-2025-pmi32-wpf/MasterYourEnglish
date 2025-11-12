using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Services
{
    public class FlashcardService : IFlashcardService
    {
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly IRepository<SavedFlashcard> _savedFlashcardRepository;

        public FlashcardService(
            IFlashcardRepository flashcardRepository,
            IRepository<SavedFlashcard> savedFlashcardRepository)
        {
            _flashcardRepository = flashcardRepository;
            _savedFlashcardRepository = savedFlashcardRepository;
        }

        public async Task<IEnumerable<SavedFlashcardDto>> GetSavedFlashcardsAsync(int userId, string searchTerm, string sortBy, bool ascending)
        {
            var flashcards = await _flashcardRepository.GetSavedFlashcardsForUserAsync(userId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerSearch = searchTerm.ToLower();
                flashcards = flashcards.Where(f => f.Word.ToLower().Contains(lowerSearch) ||
                                                 (f.Meaning ?? "").ToLower().Contains(lowerSearch));
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
                PartOfSpeech = f.PartOfSpeech ?? "",
                Transcription = f.Transcription ?? "",
                Meaning = f.Meaning ?? "",
                Example = f.Example ?? ""
            });
        }

        public async Task<bool> RemoveFromSavedAsync(int userId, int flashcardId)
        {
            var savedEntry = await _savedFlashcardRepository.FindAsync(
                sf => sf.UserId == userId && sf.FlashcardId == flashcardId
            );

            if (savedEntry.FirstOrDefault() != null)
            {
                _savedFlashcardRepository.DeleteAsync(savedEntry.First());
                return true;
            }
            return false;
        }

        public async Task<bool> AddToSavedAsync(int userId, int flashcardId)
        {
            var existing = await _savedFlashcardRepository.FindAsync(
                sf => sf.UserId == userId && sf.FlashcardId == flashcardId
            );

            if (existing.Any())
            {
                return true;
            }

            var newSavedFlashcard = new SavedFlashcard
            {
                UserId = userId,
                FlashcardId = flashcardId
            };

            await _savedFlashcardRepository.AddAsync(newSavedFlashcard);

            return true;
        }

        public async Task<List<FlashcardSessionDto>> GetSavedFlashcardsForSessionAsync(int userId)
        {
            var flashcards = await _flashcardRepository.GetSavedFlashcardsForUserAsync(userId);

            if (flashcards == null)
            {
                return new List<FlashcardSessionDto>();
            }

            return flashcards.Select(f => new FlashcardSessionDto
            {
                FlashcardId = f.FlashcardId,
                Word = f.Word,
                Transcription = f.Transcription ?? "",
                Definition = f.Meaning ?? "",
                Example = f.Example ?? "",
                PartOfSpeech = f.PartOfSpeech ?? ""
            })
            .ToList();
        }
    }
}