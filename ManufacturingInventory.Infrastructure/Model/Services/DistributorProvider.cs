using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Services {
    public class DistributorProvider : IEntityProvider<Distributor> {
        public Distributor GetEntity(Expression<Func<Distributor, bool>> expression) => throw new NotImplementedException();
        public Task<Distributor> GetEntityAsync(Expression<Func<Distributor, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Distributor> GetEntityList(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Distributor>> GetEntityListAsync(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
    }
}
