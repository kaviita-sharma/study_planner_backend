using Microsoft.AspNetCore.Mvc;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;

namespace StudyPlanner.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubTopicsController : ControllerBase
    {
        private readonly ISubTopicService _subTopicService;

        public SubTopicsController(ISubTopicService subTopicService)
        {
            _subTopicService = subTopicService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubTopicById(int id)
        {
            var topic = await _subTopicService.GetSubTopicByIdAsync(id);
            if (topic == null)
                return NotFound(new { error = "Topic not found" });

            return Ok(topic);
        }

        [HttpGet("{TopicId}")]
        public async Task<IActionResult> GetAllSubTopicByTopicId(int subjectId)
        {
            var topic = await _subTopicService.GetSubTopicByTopicId(subjectId);
            if (topic == null)
                return NotFound(new { error = $"Topic {subjectId} not found" });

            return Ok(topic);

        }
        [HttpPost]
        public async Task<IActionResult> AddTopic([FromBody] SubTopics subTopicDto)
        {
            var topicId = await _subTopicService.AddSubTopicAsync(subTopicDto);
            return Ok(new { topicId, message = "Topic added successfully" });
        }

    }
}
