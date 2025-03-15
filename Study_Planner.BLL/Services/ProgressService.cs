using Study_Planner._DLL.IRepository;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;
using Study_Planner.Core.DTOs.Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.BLL.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressRepository _repository;

        public ProgressService(IProgressRepository repository)
        {
            _repository = repository;
        }
        public IEnumerable<EnrichedProgressDTO> GetAllEnrichedProgress()
        {
            return _repository.GetAllEnrichedProgress();
        }

        public IEnumerable<ProgressDTO> GetAllProgress()
        {
            return _repository.GetAllProgress();
        }

        public List<ProgressDTO> GetProgressByUserId(int userId)
        {
            return _repository.GetProgressByUserId(userId);
        }

        public int CreateProgress(ProgressDTO progress)
        {
            return _repository.CreateProgress(progress);
        }

        public bool UpdateProgress(int id, ProgressDTO progress)
        {
            return _repository.UpdateProgress(id, progress);
        }

        public bool DeleteProgress(int id)
        {
            return _repository.DeleteProgress(id);
        }
        public EnrichedStudyPlanDTO GetUserStudyPlan(int userId)
        {
            var userPreferences = _repository.GetUserPreferences(userId);
            var progressRecords = _repository.GetUserProgressWithDetails(userId);
            var availableTimeSlots = _repository.GetUserTimeSlots(userId);

            if (userPreferences == null || progressRecords == null || availableTimeSlots == null)
            {
                return null;
            }

            return new EnrichedStudyPlanDTO
            {
                UserId = userId,
                Preferences = new UserPreferenceDTO
                {
                    LearningStyle = userPreferences.LearningStyle,
                    PreferredStudyTime = userPreferences.PreferredStudyTime,
                    StudySessionDuration = userPreferences.StudySessionDuration,
                    BreakDuration = userPreferences.BreakDuration
                },
                AvailableTimeSlots = availableTimeSlots.Select(slot => new TimeSlotDTO
                {
                    Day = slot.Day,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime
                }).ToList(),
                Subjects = progressRecords.Select(progress => new SubjectDetailsDTO
                {
                    SubjectId = progress.SubjectId,
                    SubjectName = progress.SubjectName,
                    Priority = MapPriority(progress.Priority),
                    DifficultyLevel = MapDifficulty(progress.DifficultyLevel),
                    EstimatedCompletionHours = progress.EstimatedCompletionTime
                }).ToList()
            };
        }

        private string MapPriority(int priority) => priority switch
        {
            >= 8 => "High",
            >= 5 => "Medium",
            _ => "Low"
        };

        private string MapDifficulty(int difficulty) => difficulty switch
        {
            <= 3 => "Easy",
            <= 6 => "Medium",
            _ => "Hard"
        };
    }
}
