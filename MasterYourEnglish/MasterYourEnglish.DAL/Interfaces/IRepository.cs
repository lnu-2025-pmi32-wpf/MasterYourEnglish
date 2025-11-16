namespace MasterYourEnglish.DAL.Interfaces
{
    using System.Linq.Expressions;

    public interface IRepository<T>
        where T : class
    {
        Task<T> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);
    }
}
