using Microsoft.Data.SqlClient;
using Study_Planner.Core.DTOs;
using Study_Planner.Core.DTOs.Study_Planner.Core.DTOs;

namespace Study_Planner._DLL.IRepository
{
    public interface IProgressRepository
    {
        IEnumerable<EnrichedProgressDTO> GetAllEnrichedProgress();
        IEnumerable<ProgressDTO> GetAllProgress();
        List<ProgressDTO> GetProgressByUserId(int userId);
        int CreateProgress(ProgressDTO progress);
        bool UpdateProgress(int id, ProgressDTO progress);
        bool DeleteProgress(int id);
        UserPreferenceDTO GetUserPreferences(int userId);
        List<TimeSlotDTO> GetUserTimeSlots(int userId);
        List<ProgressWithDetailsDTO> GetUserProgressWithDetails(int userId);
    }
}
