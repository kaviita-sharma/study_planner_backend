using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.BLL.IServices
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync(string token);
        Task<UserProfileResponse> GetUserProfileAsync(int userId);
        Task<AuthResponse> UpdateUserProfileAsync(int userId, UpdateProfileRequest request);
    }

}
