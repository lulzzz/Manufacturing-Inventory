using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class ContactRepository : IRepository<Contact> {
        private ManufacturingContext _context;

        public ContactRepository(ManufacturingContext context) {
            this._context = context;
        }

        public Contact Add(Contact entity) {
            return this._context.Update(entity).Entity;
        }

        public async Task<Contact> AddAsync(Contact entity) {
            return (await Task.Run(() => this._context.Update(entity))).Entity;
        }

        public Contact Update(Contact entity) {
            return this._context.Update(entity).Entity;
        }

        public async Task<Contact> UpdateAsync(Contact entity) {
            return (await Task.Run(() => this._context.Update(entity))).Entity;
        }

        public Contact Delete(Contact entity) {
            return this._context.Remove(entity).Entity;
        }

        public async Task<Contact> DeleteAsync(Contact entity) {
           return await Task.Run(() => this._context.Remove(entity).Entity);
        }

        public Contact GetEntity(Expression<Func<Contact, bool>> expression) {
            return this._context.Contacts.Include(e => e.Distributor).Include(e => e.Manufacturer).FirstOrDefault(expression);
        }

        public async Task<Contact> GetEntityAsync(Expression<Func<Contact, bool>> expression) {
            return await this._context.Contacts.Include(e => e.Distributor).Include(e => e.Manufacturer).FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Contact> GetEntityList(Expression<Func<Contact, bool>> expression = null, Func<IQueryable<Contact>, IOrderedQueryable<Contact>> orderBy = null) {
            IQueryable<Contact> query = this._context.Set<Contact>().Include(e => e.Distributor).Include(e => e.Manufacturer).AsNoTracking();

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Contact>> GetEntityListAsync(Expression<Func<Contact, bool>> expression = null, Func<IQueryable<Contact>, IOrderedQueryable<Contact>> orderBy = null) {
            IQueryable<Contact> query = this._context.Set<Contact>().Include(e => e.Distributor).Include(e => e.Manufacturer).AsNoTracking();

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return await orderBy(query).ToListAsync();
            } else {
                return await query.ToListAsync();
            }
        }

        public void Load() {
            this._context.Contacts.Include(e => e.Distributor).Include(e => e.Manufacturer).Load();
        }

        public async Task LoadAsync() {
            await this._context.Contacts.Include(e => e.Distributor).Include(e => e.Manufacturer).LoadAsync();
        }
    }
}
