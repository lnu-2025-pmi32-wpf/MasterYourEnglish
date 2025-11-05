using Microsoft.EntityFrameworkCore;
using MasterYourEnglish.DAL.Data;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;

namespace MasterYourEnglish.DAL.Repositories
{
    public class FlashcardRepository : Repository<Flashcard>, IFlashcardRepository
    {
        public FlashcardRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Flashcard>> GetFlashcardsByDifficultyLevel(string difficultyLevel)
        {
            return await _dbSet
                .Where(f => f.DifficultyLevel == difficultyLevel)
                .OrderBy(f => f.Word)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flashcard>> GetFlashcardsByTopic(int topicId)
        {
            return await _dbSet
                .Where(f => f.TopicId == topicId)
                .OrderBy(f => f.Word)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flashcard>> GetSavedFlashcardsByUserAsync(int userId)
        {
            return await _context.SavedFlashcards
                .Where(sf => sf.UserId == userId)
                .Select(sf => sf.Flashcard)
                .OrderBy(f => f.Word)
                .ToListAsync();
        }
    }
}