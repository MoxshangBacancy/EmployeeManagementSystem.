using Employee_Management_System.Services;
using Microsoft.AspNetCore.Mvc;
using Employee_Management_System.Data.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.Data;
using Swashbuckle.AspNetCore.Annotations;
using Employee_Management_System.Request;

namespace Employee_Management_System.Controllers
{
    [Route("Api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // User Login
        [HttpPost("Login")]
        public async Task<IActionResult> AuthenticateUser([FromBody] Request.LoginRequest loginRequest)
        {
            try
            {
                var user = await _authService.ValidateUserAsync(loginRequest.Email, loginRequest.Password);
                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var accessToken = _authService.GenerateJwtToken(user);
                var refreshToken = _authService.GenerateRefreshToken();

                await _authService.SaveRefreshTokenAsync(user, refreshToken);

                return Ok(new { accessToken, refreshToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login.");
                return StatusCode(500, new { message = "An error occurred while logging in. Please try again later." });
            }
        }

        // Refresh Access Token using Refresh Token
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshAccessToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var tokens = await _authService.RefreshAccessTokenAsync(request.RefreshToken);

                if (tokens == null)
                    return Unauthorized(new { message = "Invalid or expired refresh token" });

                return Ok(new { accessToken = tokens.Value.newAccessToken, refreshToken = tokens.Value.newRefreshToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token refresh.");
                return StatusCode(500, new { message = "An error occurred while refreshing the token. Please try again later." });
            }
        }

        // Logout
        [HttpPost("Logout")]
        public async Task<IActionResult> RevokeUserSession([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RevokeRefreshTokenAsync(request.RefreshToken);
                if (!result)
                    return BadRequest(new { message = "Invalid refresh token" });

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout.");
                return StatusCode(500, new { message = "An error occurred while logging out. Please try again later." });
            }
        }

        // Request Password Reset Token
        [HttpPost("ResetPassword/Request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            string? token = await _authService.RequestPasswordResetAsync(request.Email);
            if (token == null) return BadRequest(new { message = "Email not found." });

            return Ok(new
            {
                message = "Reset token sent to your email.",
                resetToken = token
            });
        }


        // ✅ Reset Password using Token
        [HttpPost("ResetPassword/NewPassword")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] Request.ResetPasswordRequest request)
        {
            bool result = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
            if (!result) return BadRequest(new { message = "Invalid or expired token." });

            return Ok(new { message = "Password reset successfully." });
        }
    }
}
