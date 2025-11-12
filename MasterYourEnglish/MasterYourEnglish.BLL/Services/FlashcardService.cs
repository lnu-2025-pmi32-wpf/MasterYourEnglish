using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Services
{
    public class FlashcardService : IFlashcardService
    {
        private readonly IFlashcardRepository _flashcardRepo;
        private readonly IRepository<SavedFlashcard> _savedFlashcardRepo;

        public FlashcardService(IFlashcardRepository flashcardRepo, IRepository<SavedFlashcard> savedFlashcardRepo)
        {
            _flashcardRepo = flashcardRepo;
            _savedFlashcardRepo = savedFlashcardRepo;
        }

        public async Task<IEnumerable<SavedFlashcardDto>> GetSavedFlashcardsAsync(int userId, string searchTerm, string sortBy, bool ascending)
        {
            var flashcards = await _flashcardRepo.GetSavedFlashcardsForUserAsync(userId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerSearch = searchTerm.ToLower();
                flashcards = flashcards.Where(f => f.Word.ToLower().Contains(lowerSearch) ||
                                                   (f.Meaning != null && f.Meaning.ToLower().Contains(lowerSearch)));
            }

            IOrderedEnumerable<Flashcard> sortedFlashcards;
            switch (sortBy)
            {
                case "Level":
                    sortedFlashcards = ascending
                        ? flashcards.OrderBy(f => f.DifficultyLevel)
                        : flashcards.OrderByDescending(f => f.DifficultyLevel);
                    break;
                case "Name":
                default:
                    sortedFlashcards = ascending
                        ? flashcards.OrderBy(f => f.Word)
                        : flashcards.OrderByDescending(f => f.Word);
                    break;
            }

            return sortedFlashcards.Select(f => new SavedFlashcardDto
            {
                FlashcardId = f.FlashcardId,
                Word = f.Word,
                PartOfSpeech = f.PartOfSpeech ?? "N/A",
                Difficulty = f.DifficultyLevel ?? "N/A",
                Transcription = f.Transcription ?? "",
                Meaning = f.Meaning ?? "",
                Example = f.Example ?? "",
                CategoryName = f.Topic?.Name ?? "General"
            });
        }

        public async Task RemoveFromSavedAsync(int userId, int flashcardId)
        {
            //var savedEntry = await _savedFlashcardRepo.FindAsync(
            //    sf => sf.UserId == userId && sf.FlashcardId == flashcardId
            //);

            //if (savedEntry.Any())
            //{
            //    _savedFlashcardRepo.Remove(savedEntry.First());
            //}
        }
    }
}