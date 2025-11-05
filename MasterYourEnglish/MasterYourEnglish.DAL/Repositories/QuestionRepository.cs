using Microsoft.EntityFrameworkCore;
using MasterYourEnglish.DAL.Data;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;

namespace MasterYourEnglish.DAL.Repositories
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Question>> GetQuestionsByDifficultyLevel(string difficultyLevel)
        {
            return await _dbSet
                .Where(q => q.DifficultyLevel == difficultyLevel)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetQuestionsByTopic(int topicId)
        {
            return await _dbSet
                .Where(q => q.TopicId == topicId)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }
    }
}