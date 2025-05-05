using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CandidateHub.Domain.Interfaces.Repos
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> AsQueryable(); // Returns an IQueryable of Entity Type T (deffered execution)

        Task<T?> GetByIdAsync(int id);
        
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> match);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> match, params Expression<Func<T, object>>[] includes);
        Task<T?> FindAsync(Expression<Func<T, bool>> match);
        Task<T?> FindAsync(Expression<Func<T, bool>> match, params Expression<Func<T, object>>[] includes);
      
        Task<bool> AnyAsync(Expression<Func<T, bool>> match);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> Filter(
          Expression<Func<T, bool>>? filter = null,
          Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
          string includeProperties = "",
          int? page = null,
          int? pageSize = null);
    }
}
