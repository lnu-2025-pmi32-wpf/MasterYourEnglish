using MasterYourEnglish.DAL.Entities;

namespace MasterYourEnglish.DAL.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IEnumerable<Question>> GetQuestionsByDifficultyLevel(string difficultyLevel);
        Task<IEnumerable<Question>> GetQuestionsByTopic(int topicId);
    }
}
