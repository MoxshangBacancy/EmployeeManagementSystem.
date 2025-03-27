namespace Employee_Management_System.Request
{
    public class UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
    }
}
