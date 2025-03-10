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

        // CREATE
        public async Task<StudySessionDto> CreateStudySessionAsync(CreateStudySessionDto createDto)
        {
            return await _studySessionRepository.CreateStudySessionAsync(createDto);
        }

        // READ (GET ALL)
        public async Task<IEnumerable<StudySessionDto>> GetAllStudySessionsAsync()
        {
            return await _studySessionRepository.GetAllStudySessionsAsync();
        }

        // READ (GET BY ID)
        public async Task<StudySessionDto> GetStudySessionByIdAsync(int id)
        {
            var session = await _studySessionRepository.GetStudySessionByIdAsync(id);
            if (session == null)
            {
                throw new KeyNotFoundException($"Study session with ID {id} not found.");
            }
            return session;
        }

        // UPDATE (Null-Check Based)
        public async Task<StudySessionDto> UpdateStudySessionAsync(int id, UpdateStudySessionDto updateDto)
        {
            var existingSession = await _studySessionRepository.GetStudySessionByIdAsync(id);
            if (existingSession == null)
            {
                throw new KeyNotFoundException($"Study session with ID {id} not found.");
            }

            var updatedSession = await _studySessionRepository.UpdateStudySessionAsync(id, updateDto);

            if (updatedSession == null)
            {
                throw new InvalidOperationException($"Failed to update study session with ID {id}.");
            }

            return updatedSession;
        }

        // DELETE
        public async Task<bool> DeleteStudySessionAsync(int id)
        {
            var existingSession = await _studySessionRepository.GetStudySessionByIdAsync(id);
            if (existingSession == null)
            {
                throw new KeyNotFoundException($"Study session with ID {id} not found.");
            }

            var deleted = await _studySessionRepository.DeleteStudySessionAsync(id);
            if (!deleted)
            {
                throw new InvalidOperationException($"Failed to delete study session with ID {id}.");
            }

            return true;
        }
    }
}
