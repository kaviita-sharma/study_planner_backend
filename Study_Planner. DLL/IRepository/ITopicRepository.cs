using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner._DLL.IRepository
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topics>> GetAllTopicsAsync();
        Task<Topics> GetTopicByIdAsync(int id);
        Task<int> AddTopicAsync(Topics topicDto);
        Task<bool> UpdateTopicAsync(int id, Topics topicDto);
        Task<bool> DeleteTopicAsync(int id);
        Task<bool> SubjectExistsAsync(int subjectId);
        Task<bool> TopicExistsAsync(string topicName, int subjectId);
        Task<bool> TopicExistsAsyncById(int id);
        Task<bool> SubTopicExistsAsync(string subTopicName, int topicId);
        Task<List<Topics>> GetTopicBySubjectId(int subjectId);
    }
}
