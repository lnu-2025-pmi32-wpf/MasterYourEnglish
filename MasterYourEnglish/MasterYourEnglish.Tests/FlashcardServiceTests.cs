using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterYourEnglish.BLL.Services;
using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using Moq;
using Xunit;

namespace MasterYourEnglish.Tests
{
    public class FlashcardServiceTests
    {
        private readonly Mock<IFlashcardRepository> flashcardRepoMock = new();
        private readonly Mock<IRepository<SavedFlashcard>> savedRepoMock = new();
        private readonly FlashcardService service;

        public FlashcardServiceTests()
        {
            service = new FlashcardService(flashcardRepoMock.Object, savedRepoMock.Object);
        }

        [Fact]
        public async Task GetSavedFlashcardsAsync_ReturnsFilteredAndMapped()
        {

            var flashcards = new List<Flashcard>
            {
                new Flashcard { FlashcardId = 1, Word = "Apple", Meaning = "Fruit", DifficultyLevel = "Easy" },
                new Flashcard { FlashcardId = 2, Word = "Banana", Meaning = "Fruit", DifficultyLevel = "Medium" }
            };
            flashcardRepoMock.Setup(r => r.GetSavedFlashcardsForUserAsync(1)).ReturnsAsync(flashcards);

            var result = await service.GetSavedFlashcardsAsync(1, "Apple", "Name", true);

            Assert.Single(result);
            Assert.Equal("Apple", result.First().Word);
        }

        [Fact]
        public async Task AddToSavedAsync_WhenNotExists_AddsAndReturnsTrue()
        {
            savedRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<SavedFlashcard, bool>>>())).ReturnsAsync(new List<SavedFlashcard>());

            var result = await service.AddToSavedAsync(1, 1);

            Assert.True(result);
            savedRepoMock.Verify(r => r.AddAsync(It.IsAny<SavedFlashcard>()), Times.Once);
        }

        [Fact]
        public async Task RemoveFromSavedAsync_WhenExists_DeletesAndReturnsTrue()
        {
            var saved = new List<SavedFlashcard> { new SavedFlashcard { FlashcardId = 1, UserId = 1 } };
            savedRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<SavedFlashcard, bool>>>())).ReturnsAsync(saved);

            var result = await service.RemoveFromSavedAsync(1, 1);

            Assert.True(result);
            savedRepoMock.Verify(r => r.DeleteAsync(saved.First()), Times.Once);
        }

        [Fact]
        public async Task GetSavedFlashcardsForSessionAsync_ReturnsMappedSession()
        {
            var flashcards = new List<Flashcard>
            {
                new Flashcard { FlashcardId = 1, Word = "Apple", Meaning = "Fruit" }
            };
            flashcardRepoMock.Setup(r => r.GetSavedFlashcardsForUserAsync(1)).ReturnsAsync(flashcards);

            var result = await service.GetSavedFlashcardsForSessionAsync(1);

            Assert.Single(result);
            Assert.Equal("Apple", result[0].Word);
        }
    }
}
