using System.Linq.Expressions;

namespace DAL
{
    public interface IRepository<T> where T : class
    {
        Task<T?> Get(int id);
        IQueryable<T> GetAll();
        Task<T?> Find(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAll(Expression<Func<T, bool>> predicate);
        Task Add(T entity);
        Task Update(T entity);
        Task Remove(T entity);
        Task SaveChanges();
    }
}