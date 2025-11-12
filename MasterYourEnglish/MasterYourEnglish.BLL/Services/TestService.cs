using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;

        public TestService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public async Task<IEnumerable<TestCardDto>> GetPublishedTestsAsync(string searchTerm, string sortBy, bool ascending)
        {
            var tests = await _testRepository.GetPublishedTestsWithDetailsAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerSearch = searchTerm.ToLower();
                tests = tests.Where(t => t.Title.ToLower().Contains(lowerSearch) ||
                                           (t.Description != null && t.Description.ToLower().Contains(lowerSearch)) ||
                                           (t.Topic != null && t.Topic.Name.ToLower().Contains(lowerSearch)));
            }

            var sortedTests = tests;
            switch (sortBy)
            {
                case "Level":
                    sortedTests = ascending
                        ? tests.OrderBy(t => t.DifficultyLevel)
                        : tests.OrderByDescending(t => t.DifficultyLevel);
                    break;
                case "Name":
                default:
                    sortedTests = ascending
                        ? tests.OrderBy(t => t.Title)
                        : tests.OrderByDescending(t => t.Title);
                    break;
            }

            return sortedTests.Select(t => new TestCardDto
            {
                TestId = t.TestId,
                TestName = t.Title,
                CategoryName = t.Topic?.Name ?? "General",
                Description = t.Description ?? "",
                DifficultyLevel = t.DifficultyLevel ?? "N/A"
            });
        }

        public Task<IEnumerable<TestCardDto>> GetPublishedTestsByTopicAsync(int topicId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestCardDto>> GetPublishedTestsForDashboardAsync()
        {
            throw new NotImplementedException();
        }
    }
}