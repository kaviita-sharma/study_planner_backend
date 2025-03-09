using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.BLL.IServices
{
    public interface ISubjectsService
    {
        Task<int> AddSubjectWithDetailsAsync(Subjects subjectDto);
    }
}
