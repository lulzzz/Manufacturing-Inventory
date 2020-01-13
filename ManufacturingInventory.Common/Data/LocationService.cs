using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;
using ManufacturingInventory.Common.Model.DbContextExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Common.Data {
    public class LocationService:IEntityService<Location> {
        private ManufacturingContext _context;

        public LocationService(ManufacturingContext context) {
            this._context = context;
        }

        public Location Add(Location entity, bool save) {
            var location = this.GetEntity(e => e.Id == entity.Id, true);
            if (location != null) {
                return null;
            }

            var added = this._context.Locations.Add(entity).Entity;
            if (save) {
                if (this.SaveChanges()) {
                    return added;
                } else {
                    return null;
                }
            } else {
                return added;
            }

        }

        public async Task<Location> AddAsync(Location entity, bool save) {
            var location = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (location != null) {
                return null;
            }

            var added = (await this._context.Locations.AddAsync(entity)).Entity;
            if (save) {
                if (await this.SaveChangesAsync()) {
                    return added;
                } else {
                    return null;
                }
            } else {
                return added;
            }
        }

        public Location Update(Location entity, bool save) {
            var location = this.GetEntity(e => e.Id == entity.Id, true);
            if (location == null) {
                return null;
            }
            location.Set(entity);
            var updated = this._context.Locations.Update(entity).Entity;
            if (save) {
                if (this.SaveChanges()) {
                    return updated;
                } else {
                    return null;
                }
            } else {
                return updated;
            }
        }

        public async Task<Location> UpdateAsync(Location entity, bool save) {
            var location = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (location == null) {
                return null;
            }
            location.Set(entity);
            var updated = (await Task.Run(() => this._context.Locations.Update(entity).Entity));
            if (save) {
                if (await this.SaveChangesAsync()) {
                    return updated;
                } else {
                    return null;
                }
            } else {
                return updated;
            }
        }

        public Location Delete(Location entity, bool save) {
            var location = this.GetEntity(e => e.Id == entity.Id, true);
            if (location == null) {
                return null;
            }

            var removed = (Location)this._context.Locations.Remove(location).Entity;
            if (save) {
                if (this.SaveChanges()) {
                    return removed;
                } else {
                    return null;
                }
            } else {
                return removed;
            }

        }

        public async Task<Location> DeleteAsync(Location entity, bool save) {
            var location = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (location == null) {
                return null;
            }

            var removed = (await Task.Run(() => this._context.Locations.Remove(location))).Entity;
            if (save) {
                if (await this.SaveChangesAsync()) {
                    return removed;
                } else {
                    return null;
                }
            } else {
                return removed;
            }
        }

        public Location GetEntity(Expression<Func<Location, bool>> expression, bool tracking) {
            if (tracking) {
                return this._context.Locations
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefault(expression);
            } else {
                return this._context.Locations
                    .AsNoTracking()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefault(expression);
            }
        }

        public async Task<Location> GetEntityAsync(Expression<Func<Location, bool>> expression, bool tracking) {
            if (tracking) {
                return await this._context.Locations
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefaultAsync(expression);
            } else {
                return await this._context.Locations
                    .AsNoTracking()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefaultAsync(expression);
            }
        }


        public IEnumerable<Location> GetEntityList(Expression<Func<Location, bool>> expression = null, Func<IQueryable<Location>, IOrderedQueryable<Location>> orderBy = null) {
            IQueryable<Location> query = this._context.Set<Location>()
                .AsNoTracking()
                .Include(e => e.Transactions)
                .Include(e => e.ItemsAtLocation);

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }


        public async Task<IEnumerable<Location>> GetEntityListAsync(Expression<Func<Location, bool>> expression = null, Func<IQueryable<Location>, IOrderedQueryable<Location>> orderBy = null) {
            IQueryable<Location> query = this._context.Set<Location>()
                .AsNoTracking()
                .Include(e => e.ItemsAtLocation)
                .Include(e => e.Transactions);

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
            this._context.Locations.AsNoTracking()
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

        public async Task<bool> SaveChangesAsync(bool undoIfFail) {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    await this._context.SaveChangesAsync();
                }
                return true;
            } catch {
                if (undoIfFail)
                    this._context.UndoDbContext();

                return false;
            }
        }

        public bool SaveChanges(bool undoIfFail) {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    this._context.SaveChanges();
                }
                return true;
            } catch {
                if (undoIfFail)
                    this._context.UndoDbContext();

                return false;
            }
        }

        public async Task<bool> SaveChangesAsync() {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    await this._context.SaveChangesAsync();
                }
                return true;
            } catch {
                return false;
            }
        }

        public bool SaveChanges() {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    this._context.SaveChanges();
                }
                return true;
            } catch {
                return false;
            }
        }
    }
}
