using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Services {
    public class ContactProvider : IEntityProvider<Contact> {
        public Contact GetEntity(Expression<Func<Contact, bool>> expression) => throw new NotImplementedException();
        public Task<Contact> GetEntityAsync(Expression<Func<Contact, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Contact> GetEntityList(Expression<Func<Contact, bool>> expression = null, Func<IQueryable<Contact>, IOrderedQueryable<Contact>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Contact>> GetEntityListAsync(Expression<Func<Contact, bool>> expression = null, Func<IQueryable<Contact>, IOrderedQueryable<Contact>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
    }
}
