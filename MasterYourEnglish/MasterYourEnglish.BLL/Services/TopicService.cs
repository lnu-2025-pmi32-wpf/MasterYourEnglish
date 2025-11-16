namespace MasterYourEnglish.BLL.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;

    public class TopicService : ITopicService
    {
        private readonly IRepository<Topic> topicRepository;

        public TopicService(IRepository<Topic> topicRepository)
        {
            this.topicRepository = topicRepository;
        }

        public async Task<List<TopicDto>> GetAllTopicsAsync()
        {
            var topics = await this.topicRepository.GetAllAsync();
            return topics
                .Select(t => new TopicDto
                {
                    TopicId = t.TopicId,
                    Name = t.Name,
                })
                .OrderBy(t => t.Name)
                .ToList();
        }
    }
}