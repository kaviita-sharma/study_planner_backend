using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.BLL.IServices
{
    public interface IAssessmentsService
    {
        IEnumerable<AssessmentDTO> GetAllAssessments();
        AssessmentDTO GetAssessmentById(int id);
        int CreateAssessment(AssessmentDTO assessment);
        bool UpdateAssessment(int id, AssessmentDTO assessment);
        bool DeleteAssessment(int id);
    }
}
