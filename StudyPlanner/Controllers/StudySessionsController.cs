using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Study_Planner._DLL.IRepository;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;
using System.Security.Claims;

namespace StudyPlanner.Application.Controllers
{
    [Route("api/study-sessions")]
    [ApiController]
    public class StudySessionsController : ControllerBase
    {
        private readonly IStudySessionService _studySessionService;
        private readonly ITopicRepository _topicRepository;

        public StudySessionsController(IStudySessionService studySessionService,
            ITopicRepository topicRepository)
        {
            _studySessionService = studySessionService;
            _topicRepository = topicRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateStudySession([FromBody] StudySessionDto studySessionDto, [FromQuery] int? subTopicId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "Invalid token or user ID missing." });
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { Message = "Invalid user ID format." });
                }
                var createdSession = await _studySessionService.CreateStudySessionAsync(userId,studySessionDto, subTopicId);
                return CreatedAtAction(nameof(GetStudySessionById), new { id = createdSession.Id }, createdSession);
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
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
