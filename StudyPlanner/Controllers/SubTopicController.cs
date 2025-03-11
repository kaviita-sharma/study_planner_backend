using Microsoft.AspNetCore.Mvc;
using Study_Planner.BLL.IServices;
using Study_Planner.BLL.Services;
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

        [HttpGet("Topic/{TopicId}")]
        public async Task<IActionResult> GetAllSubTopicByTopicId(int TopicId)
        {
            var topic = await _subTopicService.GetSubTopicByTopicId(TopicId);
            if (topic == null)
                return NotFound(new { error = $"Topic {TopicId} not found" });

            return Ok(topic);

        }
        [HttpPost]
        public async Task<IActionResult> AddSubTopic([FromQuery] int topicId,[FromBody] SubTopics subTopicDto)
        {
            var id = await _subTopicService.AddSubTopicAsync(topicId, subTopicDto);
            return Ok(new { id, message = "Topic added successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var isDeleted = await _subTopicService.DeleteSubTopicAsync(id);
            if (!isDeleted)
                return NotFound(new { error = "SubTopic not found" });

            return Ok(new { message = "SubTopic deleted successfully" });
        }
    }
}
