using MasterYourEnglish.BLL.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Interfaces
{
    public interface ITopicService
    {
        Task<List<TopicDto>> GetAllTopicsAsync();
    }
}
