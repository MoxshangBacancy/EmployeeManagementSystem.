using System.Linq.Expressions;
using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<T?> GetByIdAsync(int id, params string[] includeProperties);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<T?> GetByEmailAsync(string email);
        Task<User> GetUserByPhoneAsync(string phone);

    }
}
