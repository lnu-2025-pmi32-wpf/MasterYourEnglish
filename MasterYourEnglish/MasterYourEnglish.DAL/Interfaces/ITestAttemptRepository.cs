using MasterYourEnglish.DAL.Entities;

namespace MasterYourEnglish.DAL.Interfaces
{
    public interface ITestAttemptRepository : IRepository<TestAttempt>
    {
        Task<IEnumerable<TestAttempt>> GetAttemptsForUserAsync(int userId);
        Task<TestAttempt> GetAttemptWithAnswersAsync(int attemptId);
    }
}
