namespace MasterYourEnglish.BLL.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.Models.DTOs;

    public interface ITopicService
    {
        Task<List<TopicDto>> GetAllTopicsAsync();
    }
}
