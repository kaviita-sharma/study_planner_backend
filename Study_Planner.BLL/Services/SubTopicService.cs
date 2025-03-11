using Study_Planner._DLL.IRepository;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.BLL.Services
{
    public class SubTopicService : ISubTopicService
    {
        private readonly ISubTopicRepository _subTopicRepository;

        public SubTopicService(ISubTopicRepository subTopicRepository)
        {
            _subTopicRepository = subTopicRepository;
        }

        public async Task<IEnumerable<SubTopics>> GetAllSubTopicsAsync()
        {
            return await _subTopicRepository.GetAllSubTopicsAsync();
        }

        public async Task<SubTopics> GetSubTopicByIdAsync(int id)
        {
            return await _subTopicRepository.GetSubTopicByIdAsync(id);
        }

        public async Task<IEnumerable<SubTopics>> GetSubTopicByTopicId(int topicId)
        {
            return await _subTopicRepository.GetSubTopicByTopicId(topicId);
        }
        public async Task<int> AddSubTopicAsync(SubTopics subTopicDto)
        {
            return await _subTopicRepository.AddSubTopicAsync(subTopicDto);
        }
    }
}
