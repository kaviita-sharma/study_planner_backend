using Microsoft.AspNetCore.Http;
using Study_Planner._DLL.IRepository;
using Study_Planner._DLL.Repository;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Study_Planner._DLL.Service
{
    public class SubjectsService : ISubjectsService
    {
        private readonly ISubjectsRepository _subjectsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubjectsService(ISubjectsRepository subjectsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _subjectsRepository = subjectsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddSubjectWithDetailsAsync(Subjects subjectDto, int uId)
        {
            string userId = uId.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authorized.");
            }

            // Validation: Ensure `status` is valid
            if (!IsValidStatus(subjectDto.Status))
            {
                throw new ArgumentException("Invalid status provided. Allowed values: 'Started', 'Inprogress', 'ToDo', 'Ended'.");
            }

            // Validate required fields
            if (string.IsNullOrEmpty(subjectDto.SubjectName))
            {
                throw new ArgumentException("SubjectName and Topic are required fields.");
            }


            // Additional Validations for Numeric Fields
            if (subjectDto.DifficultyLevel <= 0 || subjectDto.DifficultyLevel > 10)
            {
                throw new ArgumentException("DifficultyLevel must be between 1 and 10.");
            }

            if (subjectDto.Priority <= 0)
            {
                throw new ArgumentException("Priority must be greater than zero.");
            }

            if (subjectDto.EstimatedCompletionTime <= 0)
            {
                throw new ArgumentException("EstimatedCompletionTime must be greater than zero.");
            }

            if (subjectDto.Topic != null)
            {
                // Topic Validations
                if (subjectDto.Topic.OrderIndex <= 0)
                {
                    throw new ArgumentException("Topic OrderIndex must be greater than zero.");
                }

                if (subjectDto.Topic.DifficultyLevel <= 0 || subjectDto.Topic.DifficultyLevel > 10)
                {
                    throw new ArgumentException("Topic DifficultyLevel must be between 1 and 10.");
                }

                // SubTopic Validations
                if (subjectDto.Topic.SubTopics != null)
                {
                    foreach (var subtopic in subjectDto.Topic.SubTopics)
                    {
                        if (subtopic.OrderIndex <= 0)
                        {
                            throw new ArgumentException("Each SubTopic OrderIndex must be greater than zero.");
                        }

                        if (subtopic.DifficultyLevel <= 0 || subtopic.DifficultyLevel > 10)
                        {
                            throw new ArgumentException("Each SubTopic DifficultyLevel must be between 1 and 10.");
                        }

                        if (subtopic.EstimatedCompletionTime <= 0)
                        {
                            throw new ArgumentException("Each SubTopic EstimatedCompletionTime must be greater than zero.");
                        }
                    }
                }
            }


            // Pass the validated data to the repository
            return await _subjectsRepository.AddSubjectWithDetailsAsync(subjectDto, userId);
        }

        public async Task<IEnumerable<Subjects>> GetAllSubjectsAsync() =>
            await _subjectsRepository.GetAllSubjectsAsync();

        public async Task<Subjects?> GetSubjectByIdAsync(int id) =>
            await _subjectsRepository.GetSubjectByIdAsync(id);

        public async Task<bool> UpdateSubjectAsync(int id, UpdateSubjects subjectDto) =>
            await _subjectsRepository.UpdateSubjectAsync(id, subjectDto);

        public async Task<bool> DeleteSubjectAsync(int id) =>
            await _subjectsRepository.DeleteSubjectAsync(id);

        private string ExtractUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            return jsonToken?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
        }

        private bool IsValidStatus(string status)
        {
            string[] validStatuses = { "Started", "Inprogress", "ToDo", "Ended" };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }
    }
}
