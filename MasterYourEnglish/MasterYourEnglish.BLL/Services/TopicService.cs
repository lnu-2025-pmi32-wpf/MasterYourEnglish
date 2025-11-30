namespace MasterYourEnglish.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.Extensions.Logging;

    public class TopicService : ITopicService
    {
        private readonly IRepository<Topic> topicRepository;
        private readonly ILogger<TopicService> logger;

        public TopicService(IRepository<Topic> topicRepository, ILogger<TopicService> logger)
        {
            this.topicRepository = topicRepository;
            this.logger = logger;
        }

        public async Task<List<TopicDto>> GetAllTopicsAsync()
        {
            this.logger.LogInformation("Getting all topics.");
            try
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
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error occurred while getting all topics.");
                throw;
            }
        }
    }
}