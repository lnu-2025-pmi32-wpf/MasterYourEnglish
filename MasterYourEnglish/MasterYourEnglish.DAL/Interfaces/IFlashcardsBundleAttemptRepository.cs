using MasterYourEnglish.DAL.Entities;

namespace MasterYourEnglish.DAL.Interfaces
{
    public interface IFlashcardsBundleAttemptRepository : IRepository<FlashcardBundleAttempt>
    {
        Task<IEnumerable<FlashcardBundleAttempt>> GetAttemptsForUserAsync(int userId);
        Task<FlashcardBundleAttempt?> GetAttemptWithDetailsAsync(int attemptId);
    }
}
