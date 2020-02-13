using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class PriceLogRepository : IRepository<PriceLog> {
        private ManufacturingContext _context;
        
        public PriceLogRepository(ManufacturingContext context) {
            this._context = context;
        }

        public PriceLog Add(PriceLog entity) => throw new NotImplementedException();
        public Task<PriceLog> AddAsync(PriceLog entity) => throw new NotImplementedException();
        public PriceLog Delete(PriceLog entity) => throw new NotImplementedException();
        public Task<PriceLog> DeleteAsync(PriceLog entity) => throw new NotImplementedException();
        public PriceLog GetEntity(System.Linq.Expressions.Expression<Func<PriceLog, bool>> expression) => throw new NotImplementedException();
        public Task<PriceLog> GetEntityAsync(System.Linq.Expressions.Expression<Func<PriceLog, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<PriceLog> GetEntityList(System.Linq.Expressions.Expression<Func<PriceLog, bool>> expression = null, Func<System.Linq.IQueryable<PriceLog>, System.Linq.IOrderedQueryable<PriceLog>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<PriceLog>> GetEntityListAsync(System.Linq.Expressions.Expression<Func<PriceLog, bool>> expression = null, Func<System.Linq.IQueryable<PriceLog>, System.Linq.IOrderedQueryable<PriceLog>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
        public PriceLog Update(PriceLog entity) => throw new NotImplementedException();
        public Task<PriceLog> UpdateAsync(PriceLog entity) => throw new NotImplementedException();
    }
}
