using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner._DLL.IRepository
{
    public interface ISubjectsRepository
    {
        Task<int> AddSubjectWithDetailsAsync(Subjects subjectDto, string userId);
    }
}
