using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Services
{
    public class TopicService : ITopicService
    {
        private readonly IRepository<Topic> _topicRepository;

        public TopicService(IRepository<Topic> topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task<List<TopicDto>> GetAllTopicsAsync()
        {
            var topics = await _topicRepository.GetAllAsync();
            return topics
                .Select(t => new TopicDto
                {
                    TopicId = t.TopicId,
                    Name = t.Name
                })
                .OrderBy(t => t.Name)
                .ToList();
        }
    }
}