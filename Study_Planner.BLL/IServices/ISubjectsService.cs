﻿using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.BLL.IServices
{
    public interface ISubjectsService
    {
        Task<int> AddSubjectWithDetailsAsync(Subjects subjectDto, int userId);
        Task<IEnumerable<Subjects>> GetAllSubjectsAsync();
        Task<Subjects?> GetSubjectByIdAsync(int id);
        Task<bool> UpdateSubjectAsync(int id, UpdateSubjects subjectDto);
        Task<bool> DeleteSubjectAsync(int id);
    }
}
