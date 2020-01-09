using ManufacturingInventory.Common.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.PartsManagment.Internal.Services {
    public interface IEntityService<T> where T:class {
        Task LoadAsync();
        void Load();
        T Update(T entity,bool save);
        Task<T> UpdateAsync(T entity,bool save);
        T Add(T entity,bool save);
        Task<T> AddAsync(T entity,bool save);
        T Delete(T entity,bool save);
        Task<T> DeleteAsync(T entity,bool save);
        T GetPartInstance(Expression<Func<T, bool>> expression);
        Task<T> GetPartInstanceAsync(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetPartInstanceList(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<IEnumerable<T>> GetPartInstanceListAsync(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<bool> SaveChangesAsync();
        bool SaveChanges();
    }
}
