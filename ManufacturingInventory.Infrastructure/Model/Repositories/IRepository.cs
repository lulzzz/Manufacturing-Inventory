using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public interface IRepository<T> {
        Task LoadAsync();
        void Load();
        T Update(T entity);
        Task<T> UpdateAsync(T entity);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        T Delete(T entity);
        Task<T> DeleteAsync(T entity);
        T GetEntity(Expression<Func<T, bool>> expression);
        Task<T> GetEntityAsync(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetEntityList(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<IEnumerable<T>> GetEntityListAsync(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
    }
}
