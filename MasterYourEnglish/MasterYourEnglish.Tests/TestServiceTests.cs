using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterYourEnglish.BLL.Services;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using Moq;
using Xunit;

namespace MasterYourEnglish.Tests
{
    public class TestServiceTests
    {
        private readonly Mock<ITestRepository> testRepoMock = new();
        private readonly TestService service;

        public TestServiceTests()
        {
            service = new TestService(testRepoMock.Object);
        }

        [Fact]
        public async Task GetPublishedTestsAsync_ReturnsFilteredAndMappedTests()
        {
            var tests = new List<Test>
            {
                new Test { TestId = 1, Title = "Math Test", Description = "Numbers", DifficultyLevel = "Easy", Topic = new Topic { Name = "Math" } },
                new Test { TestId = 2, Title = "Science Test", Description = "Physics", DifficultyLevel = "Medium", Topic = new Topic { Name = "Science" } }
            };
            testRepoMock.Setup(r => r.GetPublishedTestsWithDetailsAsync()).ReturnsAsync(tests);

            var result = await service.GetPublishedTestsAsync("Math", "Name", true);

            Assert.Single(result);
            Assert.Equal("Math Test", result.First().TestName);
        }

        [Fact]
        public async Task GetPublishedTestsAsync_SortDescendingByName_ReturnsSorted()
        {
            var tests = new List<Test>
            {
                new Test { TestId = 1, Title = "A Test", DifficultyLevel = "Easy" },
                new Test { TestId = 2, Title = "B Test", DifficultyLevel = "Medium" }
            };
            testRepoMock.Setup(r => r.GetPublishedTestsWithDetailsAsync()).ReturnsAsync(tests);

            var result = await service.GetPublishedTestsAsync("", "Name", false);

            Assert.Equal("B Test", result.First().TestName);
        }
    }
}
