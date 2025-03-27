namespace Employee_Management_System.Data.Entities
{
    public class User
    {
        public int Id { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }  
        public string Phone { get; set; } 
        public string PasswordHash { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }  
        public Role Role { get; set; }

        public string? RefreshToken { get; set; } 
        public DateTime? RefreshTokenExpiry { get; set; } 
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }

}
