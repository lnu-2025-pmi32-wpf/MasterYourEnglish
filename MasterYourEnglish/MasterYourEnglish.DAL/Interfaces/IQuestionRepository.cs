namespace MasterYourEnglish.DAL.Interfaces
{
    using MasterYourEnglish.DAL.Entities;

    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IEnumerable<Question>> GetQuestionsByDifficultyLevel(string difficultyLevel);

        Task<IEnumerable<Question>> GetQuestionsByTopic(int topicId);
    }
}
