namespace MasterYourEnglish.DAL.Interfaces
{
    using MasterYourEnglish.DAL.Entities;

    public interface ITestAttemptRepository : IRepository<TestAttempt>
    {
        Task<IEnumerable<TestAttempt>> GetAttemptsForUserAsync(int userId);

        Task<TestAttempt> GetAttemptWithAnswersAsync(int attemptId);
    }
}
