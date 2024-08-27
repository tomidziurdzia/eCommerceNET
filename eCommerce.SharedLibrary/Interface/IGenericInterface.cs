using System.Linq.Expressions;

namespace eCommerce.SharedLibrary.Interface;
using eCommerce.SharedLibrary.Response;


public interface IGenericInterface<T> where T : class
{
    Task<Response> CreateAsync(T entity);
    Task<Response> UpdateAsync(T entity);
    Task<Response> DeleteAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync(T entity);
    Task<T> FindByIdAsync(int id);
    Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
}