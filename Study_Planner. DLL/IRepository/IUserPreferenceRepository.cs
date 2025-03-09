using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner._DLL.IRepository
{
    public interface IUserPreferenceRepository
    {
        Task<UserPreference> GetByUserIdAsync(int userId);
        Task<int> CreateAsync(UserPreference preference);
        Task<bool> UpdateAsync(UserPreference preference);
        Task<bool> DeleteAsync(int id);
    }

}
