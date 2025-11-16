namespace MasterYourEnglish.DAL.Repositories
{
    using MasterYourEnglish.DAL.Data;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Question>> GetQuestionsByDifficultyLevel(string difficultyLevel)
        {
            return await this.dbSet
                .Where(q => q.DifficultyLevel == difficultyLevel)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetQuestionsByTopic(int topicId)
        {
            return await this.dbSet
                .Where(q => q.TopicId == topicId)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }
    }
}