using Microsoft.AspNetCore.Mvc;
using Study_Planner._DLL.Service;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;

namespace StudyPlanner.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectsService _subjectsService;

        public SubjectsController(ISubjectsService subjectsService)
        {
            _subjectsService = subjectsService;
        }

        [HttpPost("AddSubjectWithDetails")]
        public async Task<IActionResult> AddSubjectWithDetails([FromBody] Subjects subjectDto)
        {
            try
            {
                var result = await _subjectsService.AddSubjectWithDetailsAsync(subjectDto);
                return Ok(new { SubjectId = result, Message = "Subject added successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Error = "User not authorized. Please provide a valid token." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                // Log error for better debugging (recommended)
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Error = "An unexpected error occurred. Please try again later.",
                    Details = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _subjectsService.GetAllSubjectsAsync();
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _subjectsService.GetSubjectByIdAsync(id);
            if (subject == null) return NotFound(new { error = "Subject not found" });
            return Ok(subject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjects subjectDto)
        {
            var updated = await _subjectsService.UpdateSubjectAsync(id, subjectDto);
            if (!updated) return BadRequest(new { error = "Invalid data provided" });
            return Ok(new { message = "Subject updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var deleted = await _subjectsService.DeleteSubjectAsync(id);
            if (!deleted) return NotFound(new { error = "Subject not found" });
            return Ok(new { message = "Subject deleted successfully" });
        }
    }
}
