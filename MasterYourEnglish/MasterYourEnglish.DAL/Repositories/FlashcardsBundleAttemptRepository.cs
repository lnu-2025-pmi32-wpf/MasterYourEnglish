namespace MasterYourEnglish.DAL.Repositories
{
    using MasterYourEnglish.DAL.Data;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class FlashcardsBundleAttemptRepository : Repository<FlashcardBundleAttempt>, IFlashcardsBundleAttemptRepository
    {
        public FlashcardsBundleAttemptRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<FlashcardBundleAttempt>> GetAttemptsForUserAsync(int userId)
        {
            return await this.dbSet
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.StartedAt)
                .ToListAsync();
        }

        public async Task<FlashcardBundleAttempt?> GetAttemptWithDetailsAsync(int attemptId)
        {
            return await this.dbSet

                .Include(a => a.FlashcardsBundle)
                .Include(a => a.FlashcardsAttemptAnswers)
                .ThenInclude(ans => ans.Flashcard)
                .FirstOrDefaultAsync(a => a.AttemptId == attemptId);
        }
    }
}