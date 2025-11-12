using MasterYourEnglish.DAL.Entities;

namespace MasterYourEnglish.DAL.Interfaces
{
    public interface IFlashcardBundleRepository : IRepository<FlashcardBundle>
    {
        Task<IEnumerable<FlashcardBundle>> GetFlashcardBundlesByDifficultyLevelAsync(string difficultyLevel);
        Task<IEnumerable<FlashcardBundle>> GetFlashcardBundlesByTopicAsync(int topicId);
        Task<FlashcardBundle> GetFlashcardBundleWithDetailsAsync(int flashcardBundleId);
        Task<IEnumerable<FlashcardBundle>> GetFlashcardBundleCreatedByUserAsync(int userId);

    }
}
