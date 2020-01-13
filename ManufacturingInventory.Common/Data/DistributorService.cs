using ManufacturingInventory.Common.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace ManufacturingInventory.Common.Data {
    public class DistributorService : IEntityService<Distributor> {
        public Distributor Add(Distributor entity, bool save) => throw new NotImplementedException();
        public Task<Distributor> AddAsync(Distributor entity, bool save) => throw new NotImplementedException();
        public Distributor Delete(Distributor entity, bool save) => throw new NotImplementedException();
        public Task<Distributor> DeleteAsync(Distributor entity, bool save) => throw new NotImplementedException();
        public Distributor GetEntity(Expression<Func<Distributor, bool>> expression, bool tracking) => throw new NotImplementedException();
        public Task<Distributor> GetEntityAsync(Expression<Func<Distributor, bool>> expression, bool tracking) => throw new NotImplementedException();
        public IEnumerable<Distributor> GetEntityList(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Distributor>> GetEntityListAsync(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
        public bool SaveChanges(bool undoIfFail) => throw new NotImplementedException();
        public bool SaveChanges() => throw new NotImplementedException();
        public Task<bool> SaveChangesAsync(bool undoIfFail) => throw new NotImplementedException();
        public Task<bool> SaveChangesAsync() => throw new NotImplementedException();
        public Distributor Update(Distributor entity, bool save) => throw new NotImplementedException();
        public Task<Distributor> UpdateAsync(Distributor entity, bool save) => throw new NotImplementedException();
    }
}
