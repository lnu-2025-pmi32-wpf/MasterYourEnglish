namespace MasterYourEnglish.DAL.Interfaces
{
    using MasterYourEnglish.DAL.Entities;

    public interface IFlashcardsBundleAttemptRepository : IRepository<FlashcardBundleAttempt>
    {
        Task<IEnumerable<FlashcardBundleAttempt>> GetAttemptsForUserAsync(int userId);

        Task<FlashcardBundleAttempt?> GetAttemptWithDetailsAsync(int attemptId);
    }
}
