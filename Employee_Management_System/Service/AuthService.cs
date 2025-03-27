using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Employee_Management_System.Repository;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;
        private readonly EmailService _emailService;
        private readonly AppDbContext _context;

        public AuthService(IConfiguration configuration, IRepository<User> userRepository, EmailService emailService, AppDbContext context)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
            _context = context;
        }
        private string GenerateResetToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
        public async Task<string?> RequestPasswordResetAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null; 

            user.PasswordResetToken = GenerateResetToken();
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); 
            await _context.SaveChangesAsync();

            string resetLink = $"{_configuration["FrontendBaseUrl"]}/reset-password?token={user.PasswordResetToken}";
            await _emailService.SendEmailAsync(user.Email, "Password Reset Request",
                $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return user.PasswordResetToken;
        }
        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == token && u.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null) return false;

            user.PasswordHash = HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            return true;
        }
        public string GenerateJwtToken(User user)
        {
            if (user.Role == null) 
            {
                throw new Exception("User role is not loaded. Ensure Include(u => u.Role) is used when fetching user.");
            }

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.RoleName) 
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpireMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)); 
        }

        public async Task SaveRefreshTokenAsync(User user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpireDays"]));
            await _context.SaveChangesAsync();
        }

        public async Task<(string newAccessToken, string newRefreshToken)?> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);

            if (user == null)
                return null; 

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            await SaveRefreshTokenAsync(user, newRefreshToken);

            return (newAccessToken, newRefreshToken);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> ValidateUserAsync(string email, string enteredPassword)
        {
            var user = await _context.Users
                             .Include(u => u.Role) 
                             .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null; 

            if (user.PasswordHash.Length == 44)
            {
                Console.WriteLine("Verifying SHA-256 password...");
                string enteredSha256Hash = HashPassword(enteredPassword);
                Console.WriteLine($"Entered Password Hash: {enteredSha256Hash}");
                Console.WriteLine($"Stored Password Hash: {user.PasswordHash}");
                if (user.PasswordHash == enteredSha256Hash)
                {
                    Console.WriteLine("SHA-256 Password Verified!");
                    return user; 
                }
                else
                {
                    Console.WriteLine("Invalid credentials");
                    return null; 
                }
            }

            return user; 
        } 
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    }
