using Study_Planner.Core.DTOs;

namespace Study_Planner.BLL.IServices
{
	public interface IStudySessionService
	{
		Task<StudySessionDto> CreateStudySessionAsync(StudySessionDto studySessionDto);
		Task<IEnumerable<StudySessionDto>> GetAllStudySessionsAsync();
		Task<StudySessionDto> GetStudySessionByIdAsync(int id);
		Task<StudySessionDto> UpdateStudySessionAsync(int id, StudySessionDto studySessionDto);
		Task<bool> DeleteStudySessionAsync(int id);
	}
}
