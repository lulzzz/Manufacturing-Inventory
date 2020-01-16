using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class DistributorRepository : IRepository<Distributor> {
        public Distributor Add(Distributor entity) => throw new NotImplementedException();
        public Task<Distributor> AddAsync(Distributor entity) => throw new NotImplementedException();
        public Distributor Delete(Distributor entity) => throw new NotImplementedException();
        public Task<Distributor> DeleteAsync(Distributor entity) => throw new NotImplementedException();
        public Distributor GetEntity(Expression<Func<Distributor, bool>> expression) => throw new NotImplementedException();
        public Task<Distributor> GetEntityAsync(Expression<Func<Distributor, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Distributor> GetEntityList(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Distributor>> GetEntityListAsync(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
        public Distributor Update(Distributor entity) => throw new NotImplementedException();
        public Task<Distributor> UpdateAsync(Distributor entity) => throw new NotImplementedException();
    }
}
