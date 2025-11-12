using MasterYourEnglish.DAL.Data;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterYourEnglish.DAL.Repositories
{
    public class FlashcardBundleRepository : Repository<FlashcardBundle>, IFlashcardBundleRepository
    {
        public FlashcardBundleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<FlashcardBundle>> GetFlashcardBundlesByDifficultyLevelAsync(string difficultyLevel)
        {
            return await _dbSet
                .Where(b => b.DifficultyLevel == difficultyLevel && b.IsPublished)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<FlashcardBundle>> GetFlashcardBundlesByTopicAsync(int topicId)
        {
            return await _dbSet
                .Where(b => b.TopicId == topicId && b.IsPublished)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<FlashcardBundle> GetFlashcardBundleWithDetailsAsync(int flashcardBundleId)
        {
            return await _dbSet
                .Include(b => b.FlashcardsBundleItems)
                    .ThenInclude(fbi => fbi.Flashcard)
                .FirstAsync(b => b.FlashcardsBundleId == flashcardBundleId);
        }

        public async Task<IEnumerable<FlashcardBundle>> GetFlashcardBundleCreatedByUserAsync(int userId)
        {
            return await _dbSet
                .Where(b => b.CreatedBy == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<FlashcardBundle>> GetPublishedBundlesWithDetailsAsync()
        {
            return await _dbSet
                .Where(b => b.IsPublished)
                .Include(b => b.Topic)
                .ToListAsync();
        }
    }
}
