using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Interfaces
{
    public interface IFlashcardService
    {
        Task<IEnumerable<SavedFlashcardDto>> GetSavedFlashcardsAsync(int userId, string searchTerm, string sortBy, bool ascending);
        Task<bool> RemoveFromSavedAsync(int userId, int flashcardId);
        Task<bool> AddToSavedAsync(int userId, int flashcardId);
    }
}