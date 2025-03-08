using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Study_Planner._DLL.IRepository;
using Study_Planner.BLL.IServices;
using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserPreferenceRepository _preferenceRepository;
        private readonly IConfiguration _configuration;
        private static readonly List<string> _invalidatedTokens = new List<string>();

        public AuthService(
            IUserRepository userRepository,
            IUserPreferenceRepository preferenceRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _preferenceRepository = preferenceRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Validate email and username are unique
            if (!await _userRepository.IsEmailUniqueAsync(request.Email))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email is already in use."
                };
            }

            if (!await _userRepository.IsUsernameUniqueAsync(request.Username))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Username is already taken."
                };
            }

            // Generate salt and hash password
            string salt = GenerateSalt();
            string passwordHash = HashPassword(request.Password, salt);

            // Create user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Salt = salt,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Insert user into database
            int userId = await _userRepository.CreateAsync(user);
            user.Id = userId;

            // Create user preferences if provided
            if (request.Preferences != null)
            {
                var preferences = new UserPreference
                {
                    UserId = userId,
                    LearningStyle = request.Preferences.LearningStyle,
                    PreferredStudyTime = request.Preferences.PreferredStudyTime,
                    StudySessionDuration = request.Preferences.StudySessionDuration,
                    BreakDuration = request.Preferences.BreakDuration,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                int preferenceId = await _preferenceRepository.CreateAsync(preferences);
                preferences.Id = preferenceId;
                user.Preferences = preferences;
            }

            // Generate JWT token
            string token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful.",
                Token = token,
                User = MapUserToProfileResponse(user)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !user.IsActive)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            // Verify password
            string hashedPassword = HashPassword(request.Password, user.Salt);

            if (hashedPassword != user.PasswordHash)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            // Get user preferences
            if (user.Preferences == null)
            {
                user.Preferences = await _preferenceRepository.GetByUserIdAsync(user.Id);
            }

            // Generate JWT token
            string token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                User = MapUserToProfileResponse(user)
            };
        }

        public Task<bool> LogoutAsync(string token)
        {
            // Add token to invalidated list
            _invalidatedTokens.Add(token);
            return Task.FromResult(true);
        }

        public async Task<UserProfileResponse> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            // Ensure we have the user's preferences
            if (user.Preferences == null)
            {
                user.Preferences = await _preferenceRepository.GetByUserIdAsync(userId);
            }

            return MapUserToProfileResponse(user);
        }

        public async Task<AuthResponse> UpdateUserProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            // Check if email is changing and if it's unique
            if (request.Email != user.Email && !await _userRepository.IsEmailUniqueAsync(request.Email))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email is already in use."
                };
            }

            // Update user profile
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            bool updated = await _userRepository.UpdateAsync(user);

            // Update or create user preferences if provided
            if (request.Preferences != null)
            {
                var preferences = await _preferenceRepository.GetByUserIdAsync(userId);

                if (preferences == null)
                {
                    // Create new preferences
                    preferences = new UserPreference
                    {
                        UserId = userId,
                        LearningStyle = request.Preferences.LearningStyle,
                        PreferredStudyTime = request.Preferences.PreferredStudyTime,
                        StudySessionDuration = request.Preferences.StudySessionDuration,
                        BreakDuration = request.Preferences.BreakDuration,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _preferenceRepository.CreateAsync(preferences);
                }
                else
                {
                    // Update existing preferences
                    preferences.LearningStyle = request.Preferences.LearningStyle;
                    preferences.PreferredStudyTime = request.Preferences.PreferredStudyTime;
                    preferences.StudySessionDuration = request.Preferences.StudySessionDuration;
                    preferences.BreakDuration = request.Preferences.BreakDuration;
                    preferences.UpdatedAt = DateTime.UtcNow;

                    await _preferenceRepository.UpdateAsync(preferences);
                }

                user.Preferences = preferences;
            }

            // Generate new token with updated information
            string token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = updated,
                Message = updated ? "Profile updated successfully." : "Failed to update profile.",
                Token = token,
                User = MapUserToProfileResponse(user)
            };
        }

        #region Helper Methods

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserProfileResponse MapUserToProfileResponse(User user)
        {
            var response = new UserProfileResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };

            if (user.Preferences != null)
            {
                response.Preferences = new UserPreferenceDTO
                {
                    LearningStyle = user.Preferences.LearningStyle,
                    PreferredStudyTime = user.Preferences.PreferredStudyTime,
                    StudySessionDuration = user.Preferences.StudySessionDuration,
                    BreakDuration = user.Preferences.BreakDuration
                };
            }

            return response;
        }

    }
}
#endregion


