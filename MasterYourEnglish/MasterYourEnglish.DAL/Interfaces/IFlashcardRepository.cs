using MasterYourEnglish.DAL.Entities;

namespace MasterYourEnglish.DAL.Interfaces
{
    public interface IFlashcardRepository : IRepository<Flashcard>
    {
        Task<IEnumerable<Flashcard>> GetFlashcardsByDifficultyLevel(string difficultyLevel);
        Task<IEnumerable<Flashcard>> GetFlashcardsByTopic(int topicId);
        Task<IEnumerable<Flashcard>> GetSavedFlashcardsForUserAsync(int userId);
        Task<List<Flashcard>> GetFlashcardsByCriteriaAsync(int topicId, List<string> levels, int count);
    }
}
