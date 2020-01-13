using ManufacturingInventory.Common.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Common.Data {
    public interface IEntityService<T> where T:class {
        Task LoadAsync();
        void Load();
        T Update(T entity,bool save);
        Task<T> UpdateAsync(T entity,bool save);
        T Add(T entity,bool save);
        Task<T> AddAsync(T entity,bool save);
        T Delete(T entity,bool save);
        Task<T> DeleteAsync(T entity,bool save);

        T GetEntity(Expression<Func<T, bool>> expression,bool tracking);
        Task<T> GetEntityAsync(Expression<Func<T, bool>> expression, bool tracking);
        IEnumerable<T> GetEntityList(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<IEnumerable<T>> GetEntityListAsync(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<bool> SaveChangesAsync(bool undoIfFail);
        bool SaveChanges(bool undoIfFail);
        Task<bool> SaveChangesAsync();
        bool SaveChanges();
    }
}
