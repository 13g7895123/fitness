namespace FitnessTracker.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> FindAsync(Func<T, bool> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
}
