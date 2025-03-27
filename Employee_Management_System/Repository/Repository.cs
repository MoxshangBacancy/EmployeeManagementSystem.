using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Employee_Management_System.Data;
using Employee_Management_System.Repository;
using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)  
                .AsNoTracking()  
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id, params string[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }


        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<T?> GetByEmailAsync(string email)
        {
            var propertyInfo = typeof(T).GetProperty("Email"); 
            if (propertyInfo == null)
                throw new InvalidOperationException($"{typeof(T).Name} does not contain an 'Email' property.");

            return await _dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Email") == email);
        }
        public async Task<User> GetUserByPhoneAsync(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
        }

    }
}
