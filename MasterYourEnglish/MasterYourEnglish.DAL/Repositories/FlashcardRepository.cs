namespace MasterYourEnglish.DAL.Repositories
{
    using MasterYourEnglish.DAL.Data;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class FlashcardRepository : Repository<Flashcard>, IFlashcardRepository
    {
        public FlashcardRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Flashcard>> GetFlashcardsByDifficultyLevel(string difficultyLevel)
        {
            return await this.dbSet
                .Where(f => f.DifficultyLevel == difficultyLevel)
                .OrderBy(f => f.Word)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flashcard>> GetFlashcardsByTopic(int topicId)
        {
            return await this.dbSet
                .Where(f => f.TopicId == topicId)
                .OrderBy(f => f.Word)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flashcard>> GetSavedFlashcardsForUserAsync(int userId)
        {
            return await this.context.SavedFlashcards
                .Where(sf => sf.UserId == userId)
                .Include(sf => sf.Flashcard)
                    .ThenInclude(f => f.Topic)
                .Select(sf => sf.Flashcard)
                .ToListAsync();
        }

        public async Task<List<Flashcard>> GetFlashcardsByCriteriaAsync(int topicId, List<string> levels, int count)
        {
            IQueryable<Flashcard> query = this.dbSet.Where(f => f.TopicId == topicId);

            if (levels != null && levels.Count > 0)
            {
                query = query.Where(f => f.DifficultyLevel != null && levels.Contains(f.DifficultyLevel));
            }

            return await query
                .OrderBy(f => EF.Functions.Random())
                .Take(count)
                .ToListAsync();
        }
    }
}