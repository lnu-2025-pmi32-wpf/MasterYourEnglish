namespace MasterYourEnglish.DAL.Repositories
{
    using MasterYourEnglish.DAL.Data;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class TestAttemptRepository : Repository<TestAttempt>, ITestAttemptRepository
    {
        public TestAttemptRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<TestAttempt>> GetAttemptsForUserAsync(int userId)
        {
            return await this.dbSet
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.StartedAt)
                .ToListAsync();
        }

        public async Task<TestAttempt> GetAttemptWithAnswersAsync(int attemptId)
        {
            return await this.dbSet
                .Where(a => a.AttemptId == attemptId)
                .Include(a => a.Test)
                .Include(a => a.TestAttemptsAnswers)
                    .ThenInclude(ans => ans.Question)
                .Include(a => a.TestAttemptsAnswers)
                    .ThenInclude(ans => ans.SelectedOption)
                .Include(a => a.TestAttemptsAnswers)
                    .ThenInclude(ans => ans.SelectedOptions)
                        .ThenInclude(so => so.SelectedOption)

                .FirstAsync();
        }
    }
}