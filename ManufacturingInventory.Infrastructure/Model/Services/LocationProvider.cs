using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Providers {
    public class LocationProvider : IEntityProvider<Location> {
        private ManufacturingContext _context;

        public LocationProvider(ManufacturingContext context) => this._context = context;

        public Location GetEntity(Expression<Func<Location, bool>> expression) {

            return this._context.Locations
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .FirstOrDefault(expression);
        }

        public async Task<Location> GetEntityAsync(Expression<Func<Location, bool>> expression) {

            return await this._context.Locations
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Location> GetEntityList(Expression<Func<Location, bool>> expression = null, Func<IQueryable<Location>, IOrderedQueryable<Location>> orderBy = null) {
            IQueryable<Location> query = this._context.Set<Location>()
                .Include(e => e.Transactions)
                .Include(e => e.ItemsAtLocation)
                .AsNoTracking();

            if (expression != null) {
                query = query.AsNoTracking().Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).AsNoTracking().ToList();
            } else {
                return query.AsNoTracking().ToList();
            }
        }

        public async Task<IEnumerable<Location>> GetEntityListAsync(Expression<Func<Location, bool>> expression = null, Func<IQueryable<Location>, IOrderedQueryable<Location>> orderBy = null) {
            IQueryable<Location> query = this._context.Set<Location>()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
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
            this._context.Locations
                .Include(e => e.Transactions)
                .Include(e => e.ItemsAtLocation)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Locations.AsNoTracking()
                 .Include(e => e.Transactions)
                 .Include(e => e.ItemsAtLocation)
                 .LoadAsync();
        }
    }
}
