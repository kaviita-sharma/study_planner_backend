using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Study_Planner.BLL.IServices;
using System.Security.Claims;
using Study_Planner.Core.DTOs;

namespace StudyPlanner.Application.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Study_Planner.Core.DTOs.RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                if (!response.Success)
                {
                    return BadRequest(new { Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred during registration.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Study_Planner.Core.DTOs.LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                if (!response.Success)
                {
                    return Unauthorized(new { Message = response.Message });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred during login.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Logs out the user by invalidating the token
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var success = await _authService.LogoutAsync(token);

                if (!success)
                {
                    return BadRequest(new { Message = "Logout failed." });
                }

                return Ok(new { Message = "Logout successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred during logout.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the profile of the currently authenticated user
        /// </summary>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "Invalid token or user ID missing." });
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { Message = "Invalid user ID format." });
                }

                var userProfile = await _authService.GetUserProfileAsync(userId);

                if (userProfile == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while retrieving the user profile.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Updates the profile of the authenticated user
        /// </summary>
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "Invalid token or user ID missing." });
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { Message = "Invalid user ID format." });
                }

                var response = await _authService.UpdateUserProfileAsync(userId, request);

                if (!response.Success)
                {
                    return BadRequest(new { Message = response.Message });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while updating the profile.", Error = ex.Message });
            }
        }
    }

}
