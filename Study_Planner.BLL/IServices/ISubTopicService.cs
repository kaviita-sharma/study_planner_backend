using Study_Planner.Core.DTOs;


namespace Study_Planner.BLL.IServices
{
    public interface ISubTopicService
    {
        Task<IEnumerable<SubTopics>> GetAllSubTopicsAsync();
        Task<SubTopics> GetSubTopicByIdAsync(int id);
        Task<int> AddSubTopicAsync(int subjectId,SubTopics topicDto);
        Task<IEnumerable<SubTopics>> GetSubTopicByTopicId(int subjectId);
        Task<bool> DeleteSubTopicAsync(int id);
        Task<bool> UpdateSubTopicByIdAsync(int id, SubTopics updateDto);
    }
}
