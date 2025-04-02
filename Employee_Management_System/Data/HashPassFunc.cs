using System.Security.Cryptography;
using System.Text;

namespace Employee_Management_System.Data
{
    public class HashPassFunc
    {
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
