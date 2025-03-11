using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner._DLL.IRepository
{
    public interface ISubTopicRepository
    {
        Task<IEnumerable<SubTopics>> GetAllSubTopicsAsync();
        Task<SubTopics> GetSubTopicByIdAsync(int id);
        Task<int> AddSubTopicAsync(SubTopics subTopicDto);
        Task<IEnumerable<SubTopics>> GetSubTopicByTopicId(int topicId);
    }
}
