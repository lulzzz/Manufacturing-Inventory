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
    public class ConditionService : IEntityService<Condition> {

        private ManufacturingContext _context;

        public ConditionService(ManufacturingContext context) {
            this._context = context;
        }

        public Condition Add(Condition entity, bool save) {
            var condition=this._context.Categories.OfType<Condition>().FirstOrDefault(e => e.Id == entity.Id);
            if (condition != null) {
                return null;
            }
            var added=(Condition)this._context.Categories.Add(entity).Entity;
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

        public Condition Update(Condition entity, bool save) {
            var condition = this.GetEntity(e => e.Id == entity.Id, true);
            if (condition == null) {
                return null;
            }
            condition.Set(entity);
            var updated=this._context.Update<Condition>(condition).Entity;
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

        public async Task<Condition> UpdateAsync(Condition entity, bool save) {
            var condition = this.GetEntity(e => e.Id == entity.Id, true);
            if (condition == null) {
                return null;
            }
            condition.Set(entity);
            var updated = (await Task.Run(()=>this._context.Update<Condition>(condition))).Entity;
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

        public async Task<Condition> AddAsync(Condition entity, bool save) {
            var condition = await this._context.Categories.OfType<Condition>().FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (condition != null) {
                return null;
            }

            var added = (Condition)(await this._context.Categories.AddAsync(entity)).Entity;
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

        public Condition Delete(Condition entity, bool save) {
            var condition = this._context.Categories.OfType<Condition>().FirstOrDefault(e => e.Id == entity.Id);
            if (condition == null) {
                return null;
            }

            var removed =(Condition)this._context.Categories.Remove(condition).Entity;

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

        public async Task<Condition> DeleteAsync(Condition entity, bool save) {
            var condition = await this.GetEntityAsync(e => e.Id == entity.Id,true);
            if (condition == null) {
                return null;
            }

            var removed =(Condition)(await Task.Run(() => this._context.Categories.Remove(condition))).Entity;
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

        public Condition GetEntity(Expression<Func<Condition, bool>> expression, bool tracking) {
            if (tracking) {
                return this._context.Set<Condition>()
                    .Include(e=>e.PartInstances)
                    .FirstOrDefault(expression);
            } else {
                return this._context.Set<Condition>()
                    .AsNoTracking()
                    .Include(e => e.PartInstances)
                    .FirstOrDefault(expression);
            }
        }

        public async Task<Condition> GetEntityAsync(Expression<Func<Condition, bool>> expression, bool tracking) {
            if (tracking) {
                return await this._context.Set<Condition>()
                    .Include(e => e.PartInstances)
                    .FirstOrDefaultAsync(expression);
            } else {
                return await this._context.Set<Condition>()
                    .AsNoTracking()
                    .Include(e => e.PartInstances)
                    .FirstOrDefaultAsync(expression);
            }
        }

        public IEnumerable<Condition> GetEntityList(Expression<Func<Condition, bool>> expression = null, Func<IQueryable<Condition>, IOrderedQueryable<Condition>> orderBy = null) {
            IQueryable<Condition> query = this._context.Set<Condition>().AsNoTracking()
                .Include(e => e.PartInstances);

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Condition>> GetEntityListAsync(Expression<Func<Condition, bool>> expression = null, Func<IQueryable<Condition>, IOrderedQueryable<Condition>> orderBy = null) {
            IQueryable<Condition> query = this._context.Set<Condition>().AsNoTracking()
                .Include(e => e.PartInstances);

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
            this._context.Categories.OfType<Condition>().Include(e => e.PartInstances).Load();
        }

        public async Task LoadAsync() {
            await this._context.Categories.OfType<Condition>().Include(e => e.PartInstances).LoadAsync();
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
            {
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
}
