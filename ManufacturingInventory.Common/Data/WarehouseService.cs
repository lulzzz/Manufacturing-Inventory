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
    public class WarehouseService : IEntityService<Warehouse> {

        private ManufacturingContext _context;

        public WarehouseService(ManufacturingContext context) {
            this._context = context;
        }

        public Warehouse Add(Warehouse entity, bool save) {
            var warehouse = this.GetEntity(e => e.Id == entity.Id, true);
            if (warehouse != null) {
                return null;
            }

            var added = (Warehouse)this._context.Locations.Add(entity).Entity;
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

        public async Task<Warehouse> AddAsync(Warehouse entity, bool save) {
            var warehouse = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (warehouse != null) {
                return null;
            }

            var added = (Warehouse)(await this._context.Locations.AddAsync(entity)).Entity;
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

        public Warehouse Update(Warehouse entity, bool save) {
            var warehouse = this.GetEntity(e => e.Id == entity.Id, true);
            if (warehouse == null) {
                return null;
            }
            warehouse.Set(entity);
            var updated = (Warehouse)this._context.Locations.Update(entity).Entity;
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

        public async Task<Warehouse> UpdateAsync(Warehouse entity, bool save) {
            var warehouse = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (warehouse == null) {
                return null;
            }
            warehouse.Set(entity);
            var updated = (Warehouse)(await Task.Run(() => this._context.Locations.Update(entity).Entity));
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

        public Warehouse Delete(Warehouse entity, bool save) {
            var warehouse = this.GetEntity(e => e.Id == entity.Id, true);
            if (warehouse == null) {
                return null;
            }

            var removed = (Warehouse)this._context.Locations.Remove(warehouse).Entity;
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

        public async Task<Warehouse> DeleteAsync(Warehouse entity, bool save) {
            var warehouse = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (warehouse == null) {
                return null;
            }

            var removed = (Warehouse)(await Task.Run(() => this._context.Locations.Remove(warehouse))).Entity;
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

        public Warehouse GetEntity(Expression<Func<Warehouse, bool>> expression, bool tracking) {
            if (tracking) {
                return this._context.Set<Warehouse>()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefault(expression);
            } else {
                return this._context.Set<Warehouse>()
                    .AsNoTracking()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefault(expression);
            }
        }

        public async Task<Warehouse> GetEntityAsync(Expression<Func<Warehouse, bool>> expression, bool tracking) {
            if (tracking) {
                return await this._context.Set<Warehouse>()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefaultAsync(expression);
            } else {
                return await this._context.Set<Warehouse>()
                    .AsNoTracking()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefaultAsync(expression);
            }
        }


        public IEnumerable<Warehouse> GetEntityList(Expression<Func<Warehouse, bool>> expression = null, Func<IQueryable<Warehouse>, IOrderedQueryable<Warehouse>> orderBy = null) {
            IQueryable<Warehouse> query = this._context.Set<Warehouse>()
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


        public async Task<IEnumerable<Warehouse>> GetEntityListAsync(Expression<Func<Warehouse, bool>> expression = null, Func<IQueryable<Warehouse>, IOrderedQueryable<Warehouse>> orderBy = null) {
            IQueryable<Warehouse> query = this._context.Set<Warehouse>()
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
            this._context.Locations.OfType<Warehouse>().AsNoTracking()
                .Include(e => e.Transactions)
                .Include(e => e.ItemsAtLocation)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Locations.OfType<Warehouse>().AsNoTracking()
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
