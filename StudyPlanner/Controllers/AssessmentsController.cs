using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;
using Study_Planner._DLL.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using Study_Planner._DLL.Repository;
using Microsoft.AspNetCore.Authorization;

namespace Study_Planner.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentsController : ControllerBase
    {
        private readonly IAssessmentsService _service;
        private readonly ITopicRepository _topicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CreateAssessmentDTO> _createValidator;
        private readonly IValidator<UpdateAssessmentDTO> _updateValidator;

        public AssessmentsController(IAssessmentsService service,
            IValidator<CreateAssessmentDTO> createValidator,
            ITopicRepository topicRepository,
            IUserRepository userRepository,
            IValidator<UpdateAssessmentDTO> updateValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _topicRepository = topicRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var assessments = _service.GetAllAssessments();
                return Ok(assessments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var assessment = _service.GetAssessmentById(id);
                if (assessment == null)
                    return NotFound(new { message = "Assessment not found" });

                return Ok(assessment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssessmentDTO assessment)
        {
            var validationResult = _createValidator.Validate(assessment);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                var user = await _userRepository.GetByIdAsync(assessment.UserId);
                if (user == null)
                {
                    throw new ArgumentException("Invalid UserId. The specified user does not exist.");
                }

                if (!await _topicRepository.SubjectExistsAsync(assessment.SubjectId))
                {
                    throw new ArgumentException("Invalid SubjectId. The specified subject does not exist.");
                }

                if(!await _topicRepository.TopicExistsAsyncById(assessment.TopicId ?? -1))
                {
                    throw new ArgumentException("Invalid TopicId. The specified topic does not exist.");
                }

                var createdId = _service.CreateAssessmentAsync(assessment);
                return CreatedAtAction(nameof(GetById), new { id = createdId }, assessment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateAssessmentDTO assessment)
        {
            var validationResult = _updateValidator.Validate(assessment);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                var updated = _service.UpdateAssessment(id, assessment);
                if (!updated)
                    return NotFound(new { message = "Assessment not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var deleted = _service.DeleteAssessment(id);
                if (!deleted)
                    return NotFound(new { message = "Assessment not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}