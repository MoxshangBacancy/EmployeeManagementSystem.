namespace Employee_Management_System.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } 
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
