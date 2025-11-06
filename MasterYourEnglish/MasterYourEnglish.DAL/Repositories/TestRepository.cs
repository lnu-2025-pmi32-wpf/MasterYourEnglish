using MasterYourEnglish.DAL.Data;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MasterYourEnglish.DAL.Repositories
{
    public class TestRepository : Repository<Test>, ITestRepository
    {
        public TestRepository(ApplicationDbContext context) : base(context) {}

        public async Task<IEnumerable<Test>> GetTestsByDifficultyLevelAsync(string difficultyLevel)
        {
            return await _dbSet
                .Where(t => t.DifficultyLevel == difficultyLevel && t.IsPublished)
                .OrderBy(t => t.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Test>> GetTestsByTopicAsync(int topicId)
        {
            return await _dbSet
                .Where(t => t.TopicId == topicId && t.IsPublished)
                .Include(t => t.Topic)
                .OrderBy(t => t.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Test>> GetTestsCreatedByUserAsync(int userId)
        {
            return await _dbSet
                .Where(t => t.CreatedBy == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Test> GetTestWithDetailsAsync(int testId)
        {

            return await _dbSet
                .Include(t => t.TestQuestions)          
                    .ThenInclude(tq => tq.Question)    
                        .ThenInclude(q => q.QuestionOptions) 
                .FirstAsync(t => t.TestId == testId);
        }

        public async Task<IEnumerable<Test>> GetPublishedTestsWithTopicAsync()
        {
            return await _dbSet
                .Where(t => t.IsPublished)
                .Include(t => t.Topic) 
                .OrderBy(t => t.Title)
                .ToListAsync();
        }

    }
}
