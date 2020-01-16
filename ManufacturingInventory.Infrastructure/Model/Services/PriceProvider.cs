using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Services {
    public class PriceProvider : IEntityProvider<Price> {
        public Price GetEntity(Expression<Func<Price, bool>> expression) => throw new NotImplementedException();
        public Task<Price> GetEntityAsync(Expression<Func<Price, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Price> GetEntityList(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Price>> GetEntityListAsync(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) => throw new NotImplementedException();
    }
}
