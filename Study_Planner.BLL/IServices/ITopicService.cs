using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.BLL.IServices
{
    public interface ITopicService
    {
        Task<IEnumerable<Topics>> GetAllTopicsAsync();
        Task<Topics> GetTopicByIdAsync(int id);
        Task<int> AddTopicAsync(Topics topicDto);
        Task<bool> UpdateTopicAsync(int id, Topics topicDto);
        Task<bool> DeleteTopicAsync(int id);
    }
}
