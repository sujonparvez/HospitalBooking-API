namespace HospitalBooking.Domain.Interfaces;

using System.Linq.Expressions;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query(bool asNoTracking=true);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddMultipleAsync(List<T> entity);
    void Update(T entity);
    void UpdateMultiple(List<T> entity);
    void Delete(T entity);
    void DeleteMultiple(List<T> entity);
}
