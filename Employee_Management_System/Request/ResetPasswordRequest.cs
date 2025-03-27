namespace Employee_Management_System.Request
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
