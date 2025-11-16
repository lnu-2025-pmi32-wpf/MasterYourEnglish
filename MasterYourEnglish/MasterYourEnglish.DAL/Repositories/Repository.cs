namespace MasterYourEnglish.DAL.Repositories
{
    using System.Linq.Expressions;
    using MasterYourEnglish.DAL.Data;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class Repository<T> : IRepository<T>
        where T : class
    {
        protected readonly ApplicationDbContext context;
        protected readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await this.context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await this.context.Set<T>().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await this.context.Set<T>().AddAsync(entity);
            await this.context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await this.dbSet.Where(expression).ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            this.context.Set<T>().Update(entity);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            this.context.Set<T>().Remove(entity);
            await this.context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await this.dbSet.AddRangeAsync(entities);
        }
    }
}
