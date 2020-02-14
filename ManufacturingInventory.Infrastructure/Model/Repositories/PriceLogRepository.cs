using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class PriceLogRepository : IRepository<PriceLog> {
        private ManufacturingContext _context;
        
        public PriceLogRepository(ManufacturingContext context) {
            this._context = context;
        }

        public PriceLog Add(PriceLog entity) {
            return this._context.Add(entity).Entity;
        }

        public async Task<PriceLog> AddAsync(PriceLog entity) {
            return (await this._context.AddAsync(entity)).Entity;
        }

        public PriceLog Update(PriceLog entity) {
            var priceLog = this.GetEntity(e => (e.PriceId == entity.PriceId) && (e.PartInstanceId == entity.PartInstanceId));
            if (priceLog == null) {
                return null;
            }
            priceLog.Set(entity);
            return this._context.Update(priceLog).Entity;
        }

        public async Task<PriceLog> UpdateAsync(PriceLog entity) {
            var priceLog = await this.GetEntityAsync(e => (e.PriceId == entity.PriceId) && (e.PartInstanceId == entity.PartInstanceId));
            if (priceLog == null) {
                return null;
            }
            priceLog.Set(entity);
            return (await Task.Run(() => this._context.Update(priceLog))).Entity;
        }

        public PriceLog Delete(PriceLog entity) => throw new NotImplementedException();
        public Task<PriceLog> DeleteAsync(PriceLog entity) => throw new NotImplementedException();

        public PriceLog GetEntity(Expression<Func<PriceLog, bool>> expression) {
            return this._context.PriceLogs.Include(e => e.Price).Include(e => e.PartInstance).FirstOrDefault(expression);
        }

        public async Task<PriceLog> GetEntityAsync(Expression<Func<PriceLog, bool>> expression) {
            return await this._context.PriceLogs.Include(e => e.Price).Include(e => e.PartInstance).FirstOrDefaultAsync(expression);
        }

        public IEnumerable<PriceLog> GetEntityList(Expression<Func<PriceLog, bool>> expression = null, Func<IQueryable<PriceLog>, IOrderedQueryable<PriceLog>> orderBy = null) {
            IQueryable<PriceLog> query = this._context.Set<PriceLog>()
                .Include(e => e.Price)
                .Include(e => e.PartInstance)
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

        public async Task<IEnumerable<PriceLog>> GetEntityListAsync(Expression<Func<PriceLog, bool>> expression = null, Func<IQueryable<PriceLog>, IOrderedQueryable<PriceLog>> orderBy = null) {
            IQueryable<PriceLog> query = this._context.Set<PriceLog>()
                .Include(e => e.Price)
                .Include(e => e.PartInstance)
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
            this._context.PriceLogs
            .Include(e => e.Price)
            .Include(e => e.PartInstance)
            .Load();
        }

        public async Task LoadAsync() {
            await this._context.PriceLogs
                .Include(e => e.Price)
                .Include(e => e.PartInstance)
                .LoadAsync();
        }
    }
}
