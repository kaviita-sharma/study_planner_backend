using Study_Planner._DLL.IRepository;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;

namespace Study_Planner.BLL.Services
{
    public class StudySessionService : IStudySessionService
    {
        private readonly IStudySessionRepository _studySessionRepository;

        public StudySessionService(IStudySessionRepository studySessionRepository)
        {
            _studySessionRepository = studySessionRepository;
        }

        public async Task<StudySessionDto> CreateStudySessionAsync(int userId,StudySessionDto studySessionDto, int? subTopicId)
        {
            return await _studySessionRepository.CreateStudySessionAsync(userId, studySessionDto, subTopicId);
        }

        public async Task<IEnumerable<StudySessionDto>> GetAllStudySessionsAsync()
        {
            return await _studySessionRepository.GetAllStudySessionsAsync();
        }

        public async Task<StudySessionDto> GetStudySessionByIdAsync(int id)
        {
            return await _studySessionRepository.GetStudySessionByIdAsync(id);
        }

        public async Task<StudySessionDto> UpdateStudySessionAsync(int id, StudySessionDto studySessionDto)
        {
            var existingSession = await _studySessionRepository.GetStudySessionByIdAsync(id);
            if (existingSession == null)
            {
                return null; 
            }

            return await _studySessionRepository.UpdateStudySessionAsync(id, studySessionDto);
        }
        public async Task<bool> DeleteStudySessionAsync(int id)
        {
            return await _studySessionRepository.DeleteStudySessionAsync(id);
        }
    }
}
