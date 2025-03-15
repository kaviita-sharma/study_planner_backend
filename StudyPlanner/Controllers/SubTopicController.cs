using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetSubTopic(int id)
        {
            var subTopic = await _subTopicService.GetAllSubTopicsAsync();
            if (subTopic == null)
                return NotFound(new { error = "SubTopic not found" });

            return Ok(subTopic);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubTopicById(int id)
        {
            var subTopic = await _subTopicService.GetSubTopicByIdAsync(id);
            if (subTopic == null)
                return NotFound(new { error = "SubTopic not found" });

            return Ok(subTopic);
        }

        [HttpGet("Topic/{TopicId}")]
        public async Task<IActionResult> GetAllSubTopicByTopicId(int TopicId)
        {
            var subTopic = await _subTopicService.GetSubTopicByTopicId(TopicId);
            if (subTopic == null)
                return NotFound(new { error = $"SubTopic {TopicId} not found" });

            return Ok(subTopic);

        }
        [HttpPost("{topicId}")]
        public async Task<IActionResult> AddSubTopic(int topicId,[FromBody] SubTopics subTopicDto)
        {
            var id = await _subTopicService.AddSubTopicAsync(topicId, subTopicDto);
            return Ok(new { id, message = "SubTopic added successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var isDeleted = await _subTopicService.DeleteSubTopicAsync(id);
            if (!isDeleted)
                return NotFound(new { error = "SubTopic not found" });

            return Ok(new { message = "SubTopic deleted successfully" });
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateSubTopicByIdAsync(int id, [FromBody] SubTopics updateDto)
        {
            var res = await _subTopicService.UpdateSubTopicByIdAsync(id, updateDto);
            return Ok(new {res, message = "Updated Sucessfully"});
        }

    }
}
