using Microsoft.AspNetCore.Mvc;
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
    }
}
