using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs.Study_Planner.Core.DTOs;
using System.Security.Claims;

namespace Study_Planner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _service;

        public ProgressController(IProgressService service)
        {
            _service = service;
        }

        [HttpGet("enriched")]
        public IActionResult GetAllEnrichedProgress()
        {
            var enrichedProgressRecords = _service.GetAllEnrichedProgress();
            if (!enrichedProgressRecords.Any())
            {
                return NotFound(new { message = "No enriched progress records found." });
            }
            return Ok(enrichedProgressRecords);
        }
        [Authorize]
        [HttpGet("user-study-plan")]
        public IActionResult GetUserStudyPlan()
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

            var studyPlan = _service.GetUserStudyPlan(userId);

            if (studyPlan == null)
            {
                return NotFound(new { Message = "Study plan not found for the specified user." });
            }

            return Ok(studyPlan);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetByUserId()
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
                var progress = _service.GetProgressByUserId(userId);
                if (progress == null)
                    return NotFound();
                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProgressDTO progress)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdId = _service.CreateProgress(progress);
            return CreatedAtAction(nameof(GetByUserId), new { userId = createdId }, progress);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProgressDTO progress)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = _service.UpdateProgress(id, progress);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _service.DeleteProgress(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
