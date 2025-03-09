using Study_Planner.Core.DTOs.Study_Planner.Core.DTOs;

namespace Study_Planner._DLL.IRepository
{
    public interface IProgressRepository
    {
        IEnumerable<ProgressDTO> GetAllProgress();
        ProgressDTO GetProgressById(int id);
        int CreateProgress(ProgressDTO progress);
        bool UpdateProgress(int id, ProgressDTO progress);
        bool DeleteProgress(int id);
    }
}
