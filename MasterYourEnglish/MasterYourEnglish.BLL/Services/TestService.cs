using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Interfaces;

namespace MasterYourEnglish.BLL.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        public TestService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public async Task<IEnumerable<TestCardDto>> GetPublishedTestsForDashboardAsync()
        {
            var testEntities = await _testRepository.GetPublishedTestsWithTopicAsync();

            var testDtos = testEntities.Select(test => new TestCardDto
            {
                TestId = test.TestId,
                Title = test.Title,
                Description = test.Description,
                DifficultyLevel = test.DifficultyLevel,
                TopicName = test.Topic?.Name
            });

            return testDtos;
        }

        public async Task<IEnumerable<TestCardDto>> GetPublishedTestsByTopicAsync(int topicId)
        {
            var testEntities = await _testRepository.GetTestsByTopicAsync(topicId);

            return testEntities.Select(test => new TestCardDto
            {
                TestId = test.TestId,
                Title = test.Title,
                Description = test.Description,
                DifficultyLevel = test.DifficultyLevel,
                TopicName = test.Topic?.Name
            });
        }
    }
}