using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Services {
    public interface IEntityProvider<T> {
        T GetEntity(Expression<Func<T, bool>> expression);
        Task<T> GetEntityAsync(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetEntityList(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<IEnumerable<T>> GetEntityListAsync(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task LoadAsync();
        void Load();
    }
}
