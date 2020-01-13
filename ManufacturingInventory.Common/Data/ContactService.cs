using ManufacturingInventory.Common.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace ManufacturingInventory.Common.Data {
    public class ContactService : IEntityService<Contact> {
        public Contact Add(Contact entity, bool save) => throw new NotImplementedException();
        public Task<Contact> AddAsync(Contact entity, bool save) => throw new NotImplementedException();
        public Contact Delete(Contact entity, bool save) => throw new NotImplementedException();
        public Task<Contact> DeleteAsync(Contact entity, bool save) => throw new NotImplementedException();
        public Contact GetEntity(Expression<Func<Contact, bool>> expression, bool tracking) => throw new NotImplementedException();
        public Task<Contact> GetEntityAsync(Expression<Func<Contact, bool>> expression, bool tracking) => throw new NotImplementedException();
        public IEnumerable<Contact> GetEntityList(Expression<Func<Contact, bool>> expression = null, Func<IQueryable<Contact>, IOrderedQueryable<Contact>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Contact>> GetEntityListAsync(Expression<Func<Contact, bool>> expression = null, Func<IQueryable<Contact>, IOrderedQueryable<Contact>> orderBy = null) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public Task LoadAsync() => throw new NotImplementedException();
        public bool SaveChanges(bool undoIfFail) => throw new NotImplementedException();
        public bool SaveChanges() => throw new NotImplementedException();
        public Task<bool> SaveChangesAsync(bool undoIfFail) => throw new NotImplementedException();
        public Task<bool> SaveChangesAsync() => throw new NotImplementedException();
        public Contact Update(Contact entity, bool save) => throw new NotImplementedException();
        public Task<Contact> UpdateAsync(Contact entity, bool save) => throw new NotImplementedException();
    }
}
