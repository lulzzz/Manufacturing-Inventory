using ManufacturingInventory.Common.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace ManufacturingInventory.Common.Data {
    public class PriceService : IEntityService<Price> {
        public Price Add(Price entity, bool save) => throw new NotImplementedException();
        public Task<Price> AddAsync(Price entity, bool save) => throw new NotImplementedException();
        public Price Delete(Price entity, bool save) => throw new NotImplementedException();
        public Task<Price> DeleteAsync(Price entity, bool save) => throw new NotImplementedException();
        public Price GetEntity(Expression<Func<Price, bool>> expression, bool tracking) => throw new NotImplementedException();
        public Task<Price> GetEntityAsync(Expression<Func<Price, bool>> expression, bool tracking) => throw new NotImplementedException();
        public IEnumerable<Price> GetEntityList(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Price>> GetEntityListAsync(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
        public bool SaveChanges(bool undoIfFail) => throw new NotImplementedException();
        public bool SaveChanges() => throw new NotImplementedException();
        public Task<bool> SaveChangesAsync(bool undoIfFail) => throw new NotImplementedException();
        public Task<bool> SaveChangesAsync() => throw new NotImplementedException();
        public Price Update(Price entity, bool save) => throw new NotImplementedException();
        public Task<Price> UpdateAsync(Price entity, bool save) => throw new NotImplementedException();
    }
}
