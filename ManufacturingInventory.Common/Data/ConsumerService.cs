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
    public class ConsumerService : IEntityService<Consumer> {

        private ManufacturingContext _context;

        public ConsumerService(ManufacturingContext context) {
            this._context = context;
        }

        public Consumer Add(Consumer entity, bool save) {
            var consumer = this.GetEntity(e => e.Id == entity.Id, true);
            if (consumer != null) {
                return null;
            }

            var added = (Consumer)this._context.Locations.Add(entity).Entity;
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
        
        public async Task<Consumer> AddAsync(Consumer entity, bool save) {
            var consumer = await this.GetEntityAsync(e => e.Id == entity.Id,true);
            if (consumer != null) {
                return null;
            }

            var added = (Consumer)(await this._context.Locations.AddAsync(entity)).Entity;
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

        public Consumer Update(Consumer entity, bool save) {
            var consumer =this.GetEntity(e => e.Id == entity.Id, true);
            if (consumer == null) {
                return null;
            }
            consumer.Set(entity);
            var updated = (Consumer)this._context.Locations.Update(entity).Entity;
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

        public async Task<Consumer> UpdateAsync(Consumer entity, bool save) {
            var consumer = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (consumer == null) {
                return null;
            }
            consumer.Set(entity);
            var updated = (Consumer)(await Task.Run(()=>this._context.Locations.Update(entity).Entity));
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

        public Consumer Delete(Consumer entity, bool save) {
            var consumer = this.GetEntity(e => e.Id == entity.Id, true);
            if (consumer == null) {
                return null;
            }

            var removed = (Consumer)this._context.Locations.Remove(consumer).Entity;
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

        public async Task<Consumer> DeleteAsync(Consumer entity, bool save) {
            var consumer = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (consumer == null) {
                return null;
            }

            var removed = (Consumer)(await Task.Run(() => this._context.Locations.Remove(consumer))).Entity;
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
        
        public Consumer GetEntity(Expression<Func<Consumer, bool>> expression, bool tracking) {
            if (tracking) {
                return this._context.Set<Consumer>()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefault(expression);
            } else {
                return this._context.Set<Consumer>()
                    .AsNoTracking()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefault(expression);
            }
        }
        
        public async Task<Consumer> GetEntityAsync(Expression<Func<Consumer, bool>> expression, bool tracking) {
            if (tracking) {
                return await this._context.Set<Consumer>()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e=>e.Transactions)
                    .FirstOrDefaultAsync(expression);
            } else {
                return await this._context.Set<Consumer>()
                    .AsNoTracking()
                    .Include(e => e.ItemsAtLocation)
                    .Include(e => e.Transactions)
                    .FirstOrDefaultAsync(expression);
            }
        }
        
        
        public IEnumerable<Consumer> GetEntityList(Expression<Func<Consumer, bool>> expression = null, Func<IQueryable<Consumer>, IOrderedQueryable<Consumer>> orderBy = null) {
            IQueryable<Consumer> query = this._context.Set<Consumer>()
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
        
        
        public async Task<IEnumerable<Consumer>> GetEntityListAsync(Expression<Func<Consumer, bool>> expression = null, Func<IQueryable<Consumer>, IOrderedQueryable<Consumer>> orderBy = null) {
            IQueryable<Consumer> query = this._context.Set<Consumer>()
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
            this._context.Locations.OfType<Consumer>().AsNoTracking()
                .Include(e => e.Transactions)
                .Include(e => e.ItemsAtLocation)
                .Load();
        }

        public async Task LoadAsync() {
           await this._context.Locations.OfType<Consumer>().AsNoTracking()
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
