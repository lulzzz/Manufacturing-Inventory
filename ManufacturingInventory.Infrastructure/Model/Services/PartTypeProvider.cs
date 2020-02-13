using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Providers {
    public class PartTypeProvider : IEntityProvider<PartType> {
        private ManufacturingContext _context;

        public PartTypeProvider(ManufacturingContext context) {
            this._context = context;
        }

        public PartType GetEntity(Expression<Func<PartType, bool>> expression) => throw new NotImplementedException();
        public Task<PartType> GetEntityAsync(Expression<Func<PartType, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<PartType> GetEntityList(Expression<Func<PartType, bool>> expression = null, Func<IQueryable<PartType>, IOrderedQueryable<PartType>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<PartType>> GetEntityListAsync(Expression<Func<PartType, bool>> expression = null, Func<IQueryable<PartType>, IOrderedQueryable<PartType>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
    }
}
