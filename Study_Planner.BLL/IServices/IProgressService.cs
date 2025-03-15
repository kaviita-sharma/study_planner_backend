using Study_Planner.Core.DTOs;
using Study_Planner.Core.DTOs.Study_Planner.Core.DTOs;


namespace Study_Planner.BLL.IServices
{
    public interface IProgressService
    {
        EnrichedStudyPlanDTO GetUserStudyPlan(int userId);
        IEnumerable<EnrichedProgressDTO> GetAllEnrichedProgress();
        IEnumerable<ProgressDTO> GetAllProgress();
        List<ProgressDTO> GetProgressByUserId(int userId);
        int CreateProgress(ProgressDTO progress);
        bool UpdateProgress(int id, ProgressDTO progress);
        bool DeleteProgress(int id);
    }
}
