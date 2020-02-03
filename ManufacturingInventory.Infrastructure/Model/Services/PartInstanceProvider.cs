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
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .Include(e => e.Part)
                .FirstOrDefault(expression);
        }

        public async Task<PartInstance> GetEntityAsync(Expression<Func<PartInstance, bool>> expression) {
            return await this._context.PartInstances
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .Include(e => e.Part)
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<PartInstance> GetEntityList(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) {
            IQueryable<PartInstance> query = this._context.Set<PartInstance>()
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                .Include(e => e.Condition)
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

        public async Task<IEnumerable<PartInstance>> GetEntityListAsync(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) {
            IQueryable<PartInstance> query = this._context.Set<PartInstance>()
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                .Include(e => e.Condition)
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
            this._context.PartInstances
                 .Include(e => e.PartType)
                 .Include(e => e.CurrentLocation)
                 .Include(e => e.Price)
                 .Include(e => e.BubblerParameter)
                 .Include(e => e.Condition)
                 .Include(e => e.Part)
                 .Load();
        }

        public async Task LoadAsync() {
            await this._context.PartInstances
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .Include(e => e.Part)
                .LoadAsync();
        }
    }
}
