using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class LocationRepository : IRepository<Location> {
        private ManufacturingContext _context;

        public LocationRepository(ManufacturingContext context) {
            this._context = context;
        }

        public Location Add(Location entity) {
            var location = this.GetEntity(e => e.Id == entity.Id);
            if (location != null) {
                return null;
            }

            return this._context.Locations.Add(entity).Entity;
        }

        public async Task<Location> AddAsync(Location entity) {
            var location = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (location != null) {
                return null;
            }

            return (await this._context.Locations.AddAsync(entity)).Entity;

        }

        public Location Update(Location entity) {
            var location = this.GetEntity(e => e.Id == entity.Id);
            if (location == null) {
                return null;
            }
            location.Set(entity);
            return this._context.Locations.Update(entity).Entity;

        }

        public async Task<Location> UpdateAsync(Location entity) {
            var location = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (location == null) {
                return null;
            }
            location.Set(entity);
           return (await Task.Run(() => this._context.Update<Location>(entity).Entity));

        }

        public Location Delete(Location entity) {
            var location = this.GetEntity(e => e.Id == entity.Id);
            if (location == null) {
                return null;
            }

            return (Location)this._context.Locations.Remove(location).Entity;
        }

        public async Task<Location> DeleteAsync(Location entity) {
            var location = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (location == null) {
                return null;
            }

            return (await Task.Run(() => this._context.Locations.Remove(location))).Entity;

        }

        public Location GetEntity(Expression<Func<Location, bool>> expression) {
            return this._context.Locations
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .Include(e => ((Warehouse)e).StoredParts)
                .FirstOrDefault(expression);
        }

        public async Task<Location> GetEntityAsync(Expression<Func<Location, bool>> expression) {
            return await this._context.Locations
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .Include(e => ((Warehouse)e).StoredParts)
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Location> GetEntityList(Expression<Func<Location, bool>> expression = null, Func<IQueryable<Location>, IOrderedQueryable<Location>> orderBy = null) {
            IQueryable<Location> query = this._context.Set<Location>()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .Include(e => ((Warehouse)e).StoredParts)
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
                .Include(e => ((Warehouse)e).StoredParts)
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
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .Include(e => ((Warehouse)e).StoredParts)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Locations.AsNoTracking()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions)
                .Include(e => ((Warehouse)e).StoredParts)
                .LoadAsync();
        }
    }
}
