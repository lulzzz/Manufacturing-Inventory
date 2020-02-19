using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class PartPriceRepository : IRepository<PartPrice> {
        private ManufacturingContext _context;

        public PartPriceRepository(ManufacturingContext context) {
            this._context = context;
        }

        public PartPrice Add(PartPrice entity) {
            var part = this.GetEntity(e => e.Id == entity.Id);
            if (part != null) {
                return null;
            }
            return this._context.PartPrices.Add(entity).Entity;
        }

        public async Task<PartPrice> AddAsync(PartPrice entity) {
            return (await this._context.PartPrices.AddAsync(entity)).Entity;
        }

        public PartPrice Update(PartPrice entity) {
            var partPrice = this.GetEntity(e => (e.PartId == entity.PartId) && (e.PriceId==entity.PriceId));
            if (partPrice == null) {
                return null;
            }
            partPrice.Set(entity);
            return this._context.PartPrices.Update(partPrice).Entity;
        }

        public async Task<PartPrice> UpdateAsync(PartPrice entity) {
            var partPrice = await this.GetEntityAsync(e => (e.PartId == entity.PartId) && (e.PriceId == entity.PriceId));
            if (partPrice == null) {
                return null;
            }
            partPrice.Set(entity);
            return (await Task.Run(() => this._context.PartPrices.Update(partPrice))).Entity;
        }

        public PartPrice Delete(PartPrice entity) => throw new NotImplementedException();
        public Task<PartPrice> DeleteAsync(PartPrice entity) => throw new NotImplementedException();


        public PartPrice GetEntity(Expression<Func<PartPrice, bool>> expression) {
            return this._context.PartPrices.Include(e => e.Part).Include(e => e.Price).FirstOrDefault(expression);
        }

        public async Task<PartPrice> GetEntityAsync(Expression<Func<PartPrice, bool>> expression) {
            return await this._context.PartPrices
                .Include(e => e.Part)
                .Include(e => e.Price)
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<PartPrice> GetEntityList(Expression<Func<PartPrice, bool>> expression = null, Func<IQueryable<PartPrice>, IOrderedQueryable<PartPrice>> orderBy = null) {
            IQueryable<PartPrice> query = this._context.Set<PartPrice>()
                .Include(e => e.Part)
                .Include(e => e.Price)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<PartPrice>> GetEntityListAsync(Expression<Func<PartPrice, bool>> expression = null, Func<IQueryable<PartPrice>, IOrderedQueryable<PartPrice>> orderBy = null) {
            IQueryable<PartPrice> query = this._context.Set<PartPrice>()
                .Include(e => e.Part)
                .Include(e => e.Price)
                .AsNoTracking();

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
            this._context.PartPrices
                .Include(e => e.Part)
                .Include(e => e.Price)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.PartPrices
                .Include(e => e.Part)
                .Include(e => e.Price)
                .LoadAsync();
        }

    }
}
