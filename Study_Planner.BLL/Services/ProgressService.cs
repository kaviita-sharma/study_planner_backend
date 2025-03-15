using Study_Planner._DLL.IRepository;
using Study_Planner.BLL.IServices;
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
    }
}
