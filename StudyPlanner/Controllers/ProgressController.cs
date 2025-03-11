using Microsoft.AspNetCore.Mvc;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs.Study_Planner.Core.DTOs;

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

        [HttpGet]
        public IActionResult GetAll()
        {
            var progressRecords = _service.GetAllProgress();
            return Ok(progressRecords);
        }

        [HttpGet("{userId}")]
        public IActionResult GetByUserId(int userId)
        {
            var progress = _service.GetProgressByUserId(userId);
            if (progress == null)
                return NotFound();
            return Ok(progress);
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
