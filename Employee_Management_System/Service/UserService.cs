using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Employee_Management_System.Repository;
using Employee_Management_System.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Employee_Management_System.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly AppDbContext _context;
        public UserService(IRepository<User> userRepository, ILogger<UserService> logger, AppDbContext context)
        {
            _userRepository = userRepository;
            _logger = logger;
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId, "Role");
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                if (user.RoleId == 0)
                    throw new ArgumentException("Please assign a role.");
                if (string.IsNullOrWhiteSpace(user.FirstName))
                    throw new ArgumentException("Please enter the first name.");
                if (string.IsNullOrWhiteSpace(user.LastName))
                    throw new ArgumentException("Please enter the last name.");

                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                    throw new ArgumentException("Email already exists.");
                if (await _context.Users.AnyAsync(u => u.Phone == user.Phone))
                    throw new ArgumentException("Phone number already exists.");

                user.PasswordHash = HashPassword(user.PasswordHash);

                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                user.IsActive = true;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw new Exception("An error occurred while creating the user.");
            }
        }


       public async Task<bool> UpdateUserAsync(User user)
{
    try
    {
        if (!user.IsActive)
        {
            _logger.LogWarning($"Attempt to update inactive user {user.Id}.");
            return false; 
        }

        if (!string.IsNullOrEmpty(user.Phone))
        {
            var existingUser = await _userRepository.GetUserByPhoneAsync(user.Phone);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                _logger.LogWarning($"Phone number {user.Phone} is already in use by another user.");
                throw new InvalidOperationException("Phone number is already in use.");
            }
        }

        await _userRepository.UpdateAsync(user);
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error updating user {user.Id}.");
        return false;
    }
}


        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return false;

                await _userRepository.DeleteAsync(user.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {userId}.");
                return false;
            }
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
