using Study_Planner.Core.DTOs;
using Study_Planner._DLL.IRepository;
using Study_Planner.BLL.IServices;
using System;
using System.Collections.Generic;

namespace Study_Planner.BLL.Services
{
    public class AssessmentsService : IAssessmentsService
    {
        private readonly IAssessmentsRepository _repository;

        public AssessmentsService(IAssessmentsRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<AssessmentDTO> GetAllAssessments()
        {
            return _repository.GetAllAssessments();
        }

        public AssessmentDTO GetAssessmentById(int id)
        {
            return _repository.GetAssessmentById(id);
        }

        public Task<int> CreateAssessmentAsync(CreateAssessmentDTO assessment)
        {
            return _repository.CreateAssessmentAsync(assessment);
        }

        public bool UpdateAssessment(int id, UpdateAssessmentDTO assessment)
        {
            return _repository.UpdateAssessment(id, assessment);
        }

        public bool DeleteAssessment(int id)
        {
            return _repository.DeleteAssessment(id);
        }
    }
}
