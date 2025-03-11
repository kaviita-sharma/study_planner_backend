using Microsoft.AspNetCore.Mvc;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;

namespace StudyPlanner.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopics()
        {
            var topics = await _topicService.GetAllTopicsAsync();
            return Ok(topics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopicById(int id)
        {
            var topic = await _topicService.GetTopicByIdAsync(id);
            if (topic == null)
                return NotFound(new { error = "Topic not found" });

            return Ok(topic);
        }

        [HttpGet("{subjectId}")]
        public async Task<IActionResult> GetTopicBySubjectId(int subjectId)
        {
            var topic = await _topicService.GetTopicBySubjectId(subjectId);
            if (topic == null)
                return NotFound(new { error = $"Topic {subjectId} not found" });

            return Ok(topic);

        }
        [HttpPost]
        public async Task<IActionResult> AddTopic([FromBody] Topics topicDto)
        {
            var topicId = await _topicService.AddTopicAsync(topicDto);
            return Ok(new { topicId, message = "Topic added successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(int id, [FromBody] Topics topicDto)
        {
            var isUpdated = await _topicService.UpdateTopicAsync(id, topicDto);
            if (!isUpdated)
                return BadRequest(new { error = "Invalid data provided" });

            return Ok(new { message = "Topic updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var isDeleted = await _topicService.DeleteTopicAsync(id);
            if (!isDeleted)
                return NotFound(new { error = "Topic not found" });

            return Ok(new { message = "Topic deleted successfully" });
        }
    }
}
