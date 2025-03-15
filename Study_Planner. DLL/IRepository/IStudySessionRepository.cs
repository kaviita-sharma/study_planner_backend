using Study_Planner.Core.DTOs;

namespace Study_Planner._DLL.IRepository
{
    public interface IStudySessionRepository
    {
        Task<StudySessionDto> CreateStudySessionAsync(int userId, StudySessionDto studySessionDto, int? subTopicId);
        Task<IEnumerable<StudySessionDto>> GetAllStudySessionsAsync();
        Task<StudySessionDto> GetStudySessionByIdAsync(int id);
        Task<StudySessionDto> UpdateStudySessionAsync(int id, StudySessionDto studySessionDto);
        Task<bool> DeleteStudySessionAsync(int id);
    }
}
