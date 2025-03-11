using Study_Planner.Core.DTOs;

namespace Study_Planner._DLL.IRepository
{
    public interface IAssessmentsRepository
    {
        IEnumerable<AssessmentDTO> GetAllAssessments();
        AssessmentDTO GetAssessmentById(int id);
        Task<int> CreateAssessmentAsync(CreateAssessmentDTO assessment);
        bool UpdateAssessment(int id, UpdateAssessmentDTO assessment);
        bool DeleteAssessment(int id);
    }
}
