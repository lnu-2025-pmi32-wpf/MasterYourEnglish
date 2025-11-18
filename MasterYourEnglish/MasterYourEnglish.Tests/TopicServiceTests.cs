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
    public class TopicServiceTests
    {
        private readonly Mock<IRepository<Topic>> topicRepoMock = new();
        private readonly TopicService service;

        public TopicServiceTests()
        {
            service = new TopicService(topicRepoMock.Object);
        }

        [Fact]
        public async Task GetAllTopicsAsync_ReturnsSortedTopics()
        {
            var topics = new List<Topic>
            {
                new Topic { TopicId = 2, Name = "Zoology" },
                new Topic { TopicId = 1, Name = "Algebra" }
            };
            topicRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(topics);

            var result = await service.GetAllTopicsAsync();
            Assert.Equal(2, result.Count);
            Assert.Equal("Algebra", result[0].Name);
            Assert.Equal("Zoology", result[1].Name);
        }
    }
}
