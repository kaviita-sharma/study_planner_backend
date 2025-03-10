using Microsoft.AspNetCore.Mvc;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace StudyPlanner.Application.Controllers
{
    [Route("api/study-sessions")]
    [ApiController]
    public class StudySessionsController : ControllerBase
    {
        private readonly IStudySessionService _studySessionService;
        private readonly IValidator<CreateStudySessionDto> _createValidator;
        private readonly IValidator<UpdateStudySessionDto> _updateValidator;

        public StudySessionsController(
            IStudySessionService studySessionService,
            IValidator<CreateStudySessionDto> createValidator,
            IValidator<UpdateStudySessionDto> updateValidator)
        {
            _studySessionService = studySessionService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateStudySession([FromBody] CreateStudySessionDto createDto)
        {
            ValidationResult validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var createdSession = await _studySessionService.CreateStudySessionAsync(createDto);
            return CreatedAtAction(nameof(GetStudySessionById), new { id = createdSession.Id }, createdSession);
        }

        // GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudySessionDto>>> GetAllStudySessions()
        {
            var sessions = await _studySessionService.GetAllStudySessionsAsync();
            return Ok(sessions);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<StudySessionDto>> GetStudySessionById(int id)
        {
            try
            {
                var session = await _studySessionService.GetStudySessionByIdAsync(id);
                return Ok(session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudySession(int id, [FromBody] UpdateStudySessionDto updateDto)
        {
            ValidationResult validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                var updatedSession = await _studySessionService.UpdateStudySessionAsync(id, updateDto);
                return Ok(updatedSession);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudySession(int id)
        {
            try
            {
                await _studySessionService.DeleteStudySessionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
