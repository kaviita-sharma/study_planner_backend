using Microsoft.AspNetCore.Mvc;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;

namespace StudyPlanner.Application.Controllers
{
    [Route("api/study-sessions")]
    [ApiController]
    public class StudySessionsController : ControllerBase
    {
        private readonly IStudySessionService _studySessionService;

        public StudySessionsController(IStudySessionService studySessionService)
        {
            _studySessionService = studySessionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudySession([FromBody] StudySessionDto studySessionDto)
        {
            var createdSession = await _studySessionService.CreateStudySessionAsync(studySessionDto);
            return CreatedAtAction(nameof(GetStudySessionById), new { id = createdSession.Id }, createdSession);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudySessionDto>>> GetAllStudySessions()
        {
            var sessions = await _studySessionService.GetAllStudySessionsAsync();
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudySessionDto>> GetStudySessionById(int id)
        {
            var session = await _studySessionService.GetStudySessionByIdAsync(id);
            if (session == null)
                return NotFound();

            return Ok(session);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudySession(int id, [FromBody] StudySessionDto studySessionDto)
        {
            var updatedSession = await _studySessionService.UpdateStudySessionAsync(id, studySessionDto);
            if (updatedSession == null)
                return NotFound();

            return Ok(updatedSession);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudySession(int id)
        {
            var deleted = await _studySessionService.DeleteStudySessionAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
