using MasterYourEnglish.DAL.Entities;

namespace MasterYourEnglish.DAL.Interfaces
{
    public interface ITestRepository : IRepository<Test>
    {
        Task<IEnumerable<Test>> GetTestsByDifficultyLevelAsync(string difficultyLevel);
        Task<IEnumerable<Test>> GetTestsByTopicAsync(int topicId);
        Task<Test> GetTestWithDetailsAsync(int testId);
        Task<IEnumerable<Test>> GetTestsCreatedByUserAsync(int userId);
        Task<IEnumerable<Test>> GetPublishedTestsWithTopicAsync();

    }
}
