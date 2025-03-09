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
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;

        public TopicService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task<IEnumerable<Topics>> GetAllTopicsAsync()
        {
            return await _topicRepository.GetAllTopicsAsync();
        }

        public async Task<Topics> GetTopicByIdAsync(int id)
        {
            return await _topicRepository.GetTopicByIdAsync(id);
        }


        public async Task<int> AddTopicAsync(Topics topicDto)
        {
            // Check if SubjectId Exists
            var subjectExists = await _topicRepository.SubjectExistsAsync(topicDto.SubjectId);
            if (!subjectExists)
            {
                throw new Exception("Subject not found.");
            }

            //Check if Topic Name Already Exists
            var topicExists = await _topicRepository.TopicExistsAsync(topicDto.TopicName, topicDto.SubjectId);
            if (topicExists)
            {
                throw new Exception("Topic already exists.");
            }

            //Difficulty Level Calculation
            if (topicDto.SubTopics != null && topicDto.SubTopics.Any())
            {
                var averageDifficulty = (int)topicDto.SubTopics.Average(x => x.DifficultyLevel);
                var totalEstTime = topicDto.SubTopics.Sum(x => x.EstimatedCompletionTime);

                if (topicDto.DifficultyLevel != averageDifficulty)
                {
                    throw new Exception("Topic difficulty level must match the average difficulty of its subtopics.");
                }

                if (topicDto.EstimatedCompletionTime != totalEstTime)
                {
                    throw new Exception("Estimated completion time must match the total of subtopics' estimated completion times.");
                }

                //Check for Duplicate SubTopics
                foreach (var subTopic in topicDto.SubTopics)
                {
                    var subTopicExists = await _topicRepository.SubTopicExistsAsync(subTopic.SubTopicName, topicDto.SubjectId);
                    if (subTopicExists)
                    {
                        throw new Exception($"SubTopic '{subTopic.SubTopicName}' already exists.");
                    }
                }
            }

            // 🔹Proceed with Database Insertion
            return await _topicRepository.AddTopicAsync(topicDto);
        }

        public async Task<bool> UpdateTopicAsync(int id, Topics topicDto)
        {
            return await _topicRepository.UpdateTopicAsync(id, topicDto);
        }

        public async Task<bool> DeleteTopicAsync(int id)
        {
            return await _topicRepository.DeleteTopicAsync(id);
        }
    }
}
