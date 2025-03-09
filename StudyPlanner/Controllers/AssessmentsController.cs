using Microsoft.AspNetCore.Mvc;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;

namespace StudyPlanner.Application.Controllers
{
    
        [Route("api/[controller]")]
        [ApiController]
        public class AssessmentsController : ControllerBase
        {
        private readonly IAssessmentsService _service;

            public AssessmentsController(IAssessmentsService service)
            {
                _service = service;
            }

            [HttpGet]
            public IActionResult GetAll()
            {
                var assessments = _service.GetAllAssessments();
                return Ok(assessments);
            }

            [HttpGet("{id}")]
            public IActionResult GetById(int id)
            {
                var assessment = _service.GetAssessmentById(id);
                if (assessment == null)
                    return NotFound();
                return Ok(assessment);
            }

            [HttpPost]
            public IActionResult Create([FromBody] AssessmentDTO assessment)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdId = _service.CreateAssessment(assessment);
                return CreatedAtAction(nameof(GetById), new { id = createdId }, assessment);
            }

            [HttpPut("{id}")]
            public IActionResult Update(int id, [FromBody] AssessmentDTO assessment)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = _service.UpdateAssessment(id, assessment);
                if (!updated)
                    return NotFound();

                return NoContent();
            }

            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                var deleted = _service.DeleteAssessment(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
        }
    }

