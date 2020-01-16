using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class PriceRepository : IRepository<Price> {
        public Price Add(Price entity) => throw new NotImplementedException();
        public Task<Price> AddAsync(Price entity) => throw new NotImplementedException();
        public Price Delete(Price entity) => throw new NotImplementedException();
        public Task<Price> DeleteAsync(Price entity) => throw new NotImplementedException();
        public Price GetEntity(Expression<Func<Price, bool>> expression) => throw new NotImplementedException();
        public Task<Price> GetEntityAsync(Expression<Func<Price, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Price> GetEntityList(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Price>> GetEntityListAsync(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
        public Price Update(Price entity) => throw new NotImplementedException();
        public Task<Price> UpdateAsync(Price entity) => throw new NotImplementedException();
    }
}
