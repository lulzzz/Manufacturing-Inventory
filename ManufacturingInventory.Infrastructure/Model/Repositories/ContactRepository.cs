using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class ContactRepository : IRepository<PartInstance> {
        public PartInstance Add(PartInstance entity) => throw new NotImplementedException();
        public Task<PartInstance> AddAsync(PartInstance entity) => throw new NotImplementedException();
        public PartInstance Delete(PartInstance entity) => throw new NotImplementedException();
        public Task<PartInstance> DeleteAsync(PartInstance entity) => throw new NotImplementedException();
        public PartInstance GetEntity(Expression<Func<PartInstance, bool>> expression) => throw new NotImplementedException();
        public Task<PartInstance> GetEntityAsync(Expression<Func<PartInstance, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<PartInstance> GetEntityList(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<PartInstance>> GetEntityListAsync(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
        public PartInstance Update(PartInstance entity) => throw new NotImplementedException();
        public Task<PartInstance> UpdateAsync(PartInstance entity) => throw new NotImplementedException();
    }
}
