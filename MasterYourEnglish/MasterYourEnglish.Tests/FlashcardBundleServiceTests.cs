using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterYourEnglish.BLL.Services;
using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MasterYourEnglish.Tests
{
    public class FlashcardBundleServiceTests
    {
        private readonly Mock<IFlashcardBundleRepository> bundleRepoMock = new();
        private readonly Mock<IFlashcardsBundleAttemptRepository> attemptRepoMock = new();
        private readonly Mock<IRepository<FlashcardAttemptAnswer>> answerRepoMock = new();
        private readonly Mock<IFlashcardRepository> flashcardRepoMock = new();
        private readonly Mock<IRepository<FlashcardBundleItem>> bundleItemRepoMock = new();
        private readonly Mock<ILogger<FlashcardBundleService>> loggerMock = new();

        private readonly FlashcardBundleService service;

        public FlashcardBundleServiceTests()
        {
            service = new FlashcardBundleService(
                bundleRepoMock.Object,
                attemptRepoMock.Object,
                answerRepoMock.Object,
                flashcardRepoMock.Object,
                bundleItemRepoMock.Object,
                loggerMock.Object
            );
        }

        [Fact]
        public async Task GetPublishedBundlesAsync_ReturnsFilteredAndMappedBundles()
        {
            // Arrange
            var bundles = new List<FlashcardBundle>
            {
                new FlashcardBundle { FlashcardsBundleId = 1, Title = "Animals", Description = "Desc1", DifficultyLevel = "Easy", Topic = new Topic { Name = "Nature" } },
                new FlashcardBundle { FlashcardsBundleId = 2, Title = "Fruits", Description = "Desc2", DifficultyLevel = "Medium", Topic = new Topic { Name = "Food" } }
            };
            bundleRepoMock.Setup(r => r.GetPublishedBundlesWithDetailsAsync()).ReturnsAsync(bundles);

            // Act
            var result = await service.GetPublishedBundlesAsync("Ani", "Name", true);

            // Assert
            Assert.Single(result);
            Assert.Equal("Animals", result.First().BundleName);
        }

        [Fact]
        public async Task GetFlashcardSessionAsync_ReturnsOrderedSession()
        {
            // Arrange
            var bundle = new FlashcardBundle
            {
                FlashcardsBundleId = 1,
                FlashcardsBundleItems = new List<FlashcardBundleItem>
                {
                    new FlashcardBundleItem { Position = 2, Flashcard = new Flashcard { FlashcardId = 2, Word = "B" } },
                    new FlashcardBundleItem { Position = 1, Flashcard = new Flashcard { FlashcardId = 1, Word = "A" } }
                }
            };
            bundleRepoMock.Setup(r => r.GetFlashcardBundleWithDetailsAsync(1)).ReturnsAsync(bundle);

            // Act
            var result = await service.GetFlashcardSessionAsync(1);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("A", result[0].Word); // ordered by Position
        }
    }
}
