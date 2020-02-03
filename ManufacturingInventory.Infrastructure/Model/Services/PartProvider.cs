using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Services {
    public class PartProvider : IEntityProvider<Part> {
        private ManufacturingContext _context;

        public PartProvider(ManufacturingContext context) => this._context = context;

        public Part GetEntity(Expression<Func<Part, bool>> expression) {
            return this._context.Parts
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .FirstOrDefault(expression);
        }

        public async Task<Part> GetEntityAsync(Expression<Func<Part, bool>> expression) {
            return await this._context.Parts
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Part> GetEntityList(Expression<Func<Part, bool>> expression = null, Func<IQueryable<Part>, IOrderedQueryable<Part>> orderBy = null) {
            IQueryable<Part> query = this._context.Set<Part>()
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
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

        public async Task<IEnumerable<Part>> GetEntityListAsync(Expression<Func<Part, bool>> expression = null, Func<IQueryable<Part>, IOrderedQueryable<Part>> orderBy = null) {
            IQueryable<Part> query = this._context.Set<Part>()
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
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
            this._context.Parts
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Parts.AsNoTracking()
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .LoadAsync();
        }
    }
}
