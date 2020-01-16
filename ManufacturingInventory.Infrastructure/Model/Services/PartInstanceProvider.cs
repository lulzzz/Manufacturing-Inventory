using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Services {
    public class PartInstanceProvider : IEntityProvider<PartInstance> {
        private ManufacturingContext _context;

        public PartInstanceProvider(ManufacturingContext context) => this._context = context;

        public PartInstance GetEntity(Expression<Func<PartInstance, bool>> expression) {
            return this._context.PartInstances
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Location)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .Include(e => e.Part)
                .AsNoTracking()
                .FirstOrDefault(expression);
        }

        public async Task<PartInstance> GetEntityAsync(Expression<Func<PartInstance, bool>> expression) {
            return await this._context.PartInstances
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .Include(e => e.Part)
                .AsNoTracking()
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<PartInstance> GetEntityList(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) {
            IQueryable<PartInstance> query = this._context.Set<PartInstance>()
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Location)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                .Include(e => e.Condition)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression).AsNoTracking();
            }

            if (orderBy != null) {
                return orderBy(query).AsNoTracking().ToList();
            } else {
                return query.AsNoTracking().ToList();
            }
        }

        public async Task<IEnumerable<PartInstance>> GetEntityListAsync(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) {
            IQueryable<PartInstance> query = this._context.Set<PartInstance>()
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Location)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                .Include(e => e.Condition)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression).AsNoTracking();
            }

            if (orderBy != null) {
                return await orderBy(query).AsNoTracking().ToListAsync();
            } else {
                return await query.AsNoTracking().ToListAsync();
            }
        }

        public void Load() {
            this._context.PartInstances
                 .Include(e => e.Transactions)
                     .ThenInclude(e => e.Session)
                 .Include(e => e.Transactions)
                     .ThenInclude(e => e.Location)
                 .Include(e => e.PartType)
                 .Include(e => e.CurrentLocation)
                 .Include(e => e.Price)
                 .Include(e => e.BubblerParameter)
                 .Include(e => e.Condition)
                 .Include(e => e.Part)
                 .AsNoTracking()
                 .Load();
        }

        public async Task LoadAsync() {
            await this._context.PartInstances
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Location)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .Include(e => e.Part)
                .AsNoTracking()
                .LoadAsync();
        }
    }
}
