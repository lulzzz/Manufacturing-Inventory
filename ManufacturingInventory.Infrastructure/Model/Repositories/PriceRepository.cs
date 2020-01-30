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
        private ManufacturingContext _context;

        public PriceRepository(ManufacturingContext context) {
            this._context = context;
        }
       
        public Price Add(Price entity) {
            var price = this.GetEntity(e => e.Id == entity.Id);
            if (price != null) {
                return null;
            }
            return this._context.Add(entity).Entity;
        }
           
        public async Task<Price> AddAsync(Price entity) {
            var price = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (price != null) {
                return null;
            }
            return (await this._context.AddAsync(entity)).Entity;
        }
        
        public Price Update(Price entity) {
            var price = this.GetEntity(e => e.Id == entity.Id);
            if (price == null) {
                return null;
            }
            price.Set(entity);
            return this._context.Update(price).Entity;
        }
       
        public async Task<Price> UpdateAsync(Price entity) {
            var price =await this.GetEntityAsync(e => e.Id == entity.Id);
            if (price == null) {
                return null;
            }
            price.Set(entity);
            return this._context.Update(price).Entity;
        }
       
        
        public Price Delete(Price entity) => throw new NotImplementedException();
        public Task<Price> DeleteAsync(Price entity) => throw new NotImplementedException();

        public Price GetEntity(Expression<Func<Price, bool>> expression) {
            return this._context.Prices
                .Include(e => e.Distributor)
                .Include(e => e.Attachments)
                .FirstOrDefault(expression);
        }

        public async Task<Price> GetEntityAsync(Expression<Func<Price, bool>> expression) {
            return await this._context.Prices
                .Include(e => e.Distributor)
                .Include(e=>e.Attachments)
                .FirstOrDefaultAsync(expression);
        }


        public IEnumerable<Price> GetEntityList(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) {
            IQueryable<Price> query = this._context.Set<Price>()
                .Include(e => e.Distributor)
                .Include(e => e.Attachments)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return query.ToList();
            } else {
                return orderBy(query).ToList();
            }
        }
        
        
        public async Task<IEnumerable<Price>> GetEntityListAsync(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) {
            IQueryable<Price> query = this._context.Set<Price>()
                .Include(e => e.Distributor)
                .Include(e => e.Attachments)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return await query.ToListAsync();
            } else {
                return await orderBy(query).ToListAsync();
            }
        }
        
        public void Load() {
            this._context.Prices
                .Include(e => e.Distributor)
                .Include(e => e.Attachments)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Prices
                .Include(e => e.Distributor)
                .Include(e => e.Attachments)
                .LoadAsync();
        }
    }
}
