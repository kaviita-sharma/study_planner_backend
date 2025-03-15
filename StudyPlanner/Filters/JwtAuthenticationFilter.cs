using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Study_Planner.Application.Filters
{
    public class JwtAuthenticationFilter : IAuthorizationFilter
    {
        private readonly string _secretKey;

        public JwtAuthenticationFilter(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:Key"];
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token is missing." });
                return;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "issuer",
                    ValidAudience = "audience",
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                var userIdClaim = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    context.Result = new UnauthorizedObjectResult(new { message = "Invalid token or user ID missing." });
                    return;
                }

                context.HttpContext.Items["userId"] = int.Parse(userIdClaim); 
            }
            catch (Exception ex)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token validation failed.", error = ex.Message });
            }
        }
    }
}
