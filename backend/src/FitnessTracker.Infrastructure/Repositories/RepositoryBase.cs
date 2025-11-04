using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Repositories;

public abstract class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly FitnessTrackerDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected RepositoryBase(FitnessTrackerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<List<T>> FindAsync(Func<T, bool> predicate)
    {
        return await Task.FromResult(_dbSet.Where(predicate).ToList());
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.FromResult(entity);
    }

    public virtual Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
