using Study_Planner.Core.DTOs.Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.BLL.IServices
{
    public interface IProgressService
    {
        IEnumerable<ProgressDTO> GetAllProgress();
        ProgressDTO GetProgressById(int id);
        int CreateProgress(ProgressDTO progress);
        bool UpdateProgress(int id, ProgressDTO progress);
        bool DeleteProgress(int id);
    }
}
