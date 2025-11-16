namespace MasterYourEnglish.BLL.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Models.DTOs;

    public interface IFlashcardService
    {
        Task<IEnumerable<SavedFlashcardDto>> GetSavedFlashcardsAsync(int userId, string searchTerm, string sortBy, bool ascending);

        Task<bool> RemoveFromSavedAsync(int userId, int flashcardId);

        Task<bool> AddToSavedAsync(int userId, int flashcardId);

        Task<List<FlashcardSessionDto>> GetSavedFlashcardsForSessionAsync(int userId);
    }
}