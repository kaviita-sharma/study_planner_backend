using Study_Planner.Core.DTOs;

namespace Study_Planner.BLL.IServices
{
    public interface IStudySessionService
    {
        Task<StudySessionDto> CreateStudySessionAsync(CreateStudySessionDto createDto);
        Task<IEnumerable<StudySessionDto>> GetAllStudySessionsAsync();
        Task<StudySessionDto> GetStudySessionByIdAsync(int id);
        Task<StudySessionDto> UpdateStudySessionAsync(int id, UpdateStudySessionDto updateDto);
        Task<bool> DeleteStudySessionAsync(int id);
    }
}
