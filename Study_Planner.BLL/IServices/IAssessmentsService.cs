using Study_Planner.Core.DTOs;

namespace Study_Planner.BLL.IServices
{
    public interface IAssessmentsService
    {
        IEnumerable<AssessmentDTO> GetAllAssessments();
        AssessmentDTO GetAssessmentById(int id);
        int CreateAssessment(CreateAssessmentDTO assessment);
        bool UpdateAssessment(int id, UpdateAssessmentDTO assessment);
        bool DeleteAssessment(int id);
    }
}
