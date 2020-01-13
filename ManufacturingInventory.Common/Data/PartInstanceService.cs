using ManufacturingInventory.Common.Buisness.Interfaces;
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
    public class PartInstanceService : IEntityService<PartInstance> {
        private ManufacturingContext _context;

        public PartInstanceService(ManufacturingContext context) {
            this._context = context;
        }

        public PartInstance Add(PartInstance entity, bool save) {
            var instance = this._context.PartInstances.AsNoTracking().FirstOrDefault(e => e.Id == entity.Id);
            if (instance != null) {
                return null;
            }

            if(entity.Part!=null && entity.CurrentLocation != null && entity.Price!=null) {

                if (entity.PartType != null) {
                    this._context.Entry<PartType>(entity.PartType).State = EntityState.Modified;
                }

                if (entity.Condition != null) {
                    this._context.Entry<Condition>(entity.Condition).State = EntityState.Modified;
                }

                if (entity.BubblerParameter != null) {
                    this._context.Entry<BubblerParameter>(entity.BubblerParameter).State = EntityState.Modified;
                }
                this._context.Entry<Part>(entity.Part).State = EntityState.Modified;
                this._context.Entry<Location>(entity.CurrentLocation).State = EntityState.Modified;
                if (save) {
                    if (this.SaveChanges()) {
                        return entity;
                    } else {
                        return null;
                    }
                } else {
                    return entity;
                }
            } else {
                return entity;
            }
        }

        public PartInstance Delete(PartInstance entity, bool save) {
            var toRemove = this._context.PartInstances.FirstOrDefault(e => e.Id == entity.Id);
            if (toRemove != null) {

                var removed = this._context.PartInstances.Remove(toRemove).Entity;
                if (save) {
                    if (this.SaveChanges()) {
                        return removed;
                    } else {
                        return null;
                    }
                }
                return removed;
            } else {
                return null;
            }
        }

        public async Task<PartInstance> DeleteAsync(PartInstance entity, bool save) {
            var toRemove =await this._context.PartInstances.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (toRemove != null) {
                var removed = (await Task.Run(() => {
                    return this._context.PartInstances.Remove(toRemove);
                })).Entity;
                //var removed = this._context.PartInstances.Remove(entity).Entity;
                if (save) {
                    if (await this.SaveChangesAsync()) {
                        return removed;
                    } else {
                        return null;
                    }
                }
                return removed;
            } else {
                return null;
            }
        }

        public PartInstance Update(PartInstance entity, bool save) {
            var instance = this.GetEntity(e => e.Id == entity.Id, true);
            if (instance != null) {
                instance.Name = entity.Name;
                instance.IsBubbler = entity.IsBubbler;
                instance.Quantity = entity.Quantity;
                instance.MinQuantity = entity.MinQuantity;
                instance.SafeQuantity = entity.SafeQuantity;
                instance.SerialNumber = entity.SerialNumber;
                instance.TotalCost = entity.TotalCost;
                instance.UnitCost = entity.UnitCost;
                instance.BatchNumber = entity.BatchNumber;
                instance.CostReported = entity.CostReported;
                instance.IsResuable = entity.IsResuable;
                instance.SkuNumber = entity.SkuNumber;

                instance.ConditionId = entity.ConditionId;
                instance.LocationId = entity.LocationId;
                instance.PartTypeId = entity.PartTypeId;

                if (entity.IsBubbler && entity.BubblerParameter != null) {
                    if (!instance.BubblerParameter.Compare(entity.BubblerParameter)){
                        instance.BubblerParameter.Set(entity.BubblerParameter);
                        this._context.Entry<BubblerParameter>(instance.BubblerParameter).State = EntityState.Modified;
                    }
                }

                var updated = this._context.PartInstances.Update(instance).Entity;
                if (save) {
                    if (this.SaveChanges()) {
                        return updated;
                    } else {
                        return null;
                    }
                } else {
                    return updated;
                }
            } else {
                return null;
            }
        }

        public async Task<PartInstance> UpdateAsync(PartInstance entity, bool save) {
            var instance =await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (instance != null) {
                instance.Name = entity.Name;
                instance.IsBubbler = entity.IsBubbler;
                instance.Quantity = entity.Quantity;
                instance.MinQuantity = entity.MinQuantity;
                instance.SafeQuantity = entity.SafeQuantity;
                instance.SerialNumber = entity.SerialNumber;
                instance.TotalCost = entity.TotalCost;
                instance.UnitCost = entity.UnitCost;
                instance.BatchNumber = entity.BatchNumber;
                instance.CostReported = entity.CostReported;
                instance.IsResuable = entity.IsResuable;
                instance.SkuNumber = entity.SkuNumber;

                instance.ConditionId = entity.ConditionId;
                instance.LocationId = entity.LocationId;
                instance.PartTypeId = entity.PartTypeId;

                if (entity.IsBubbler && entity.BubblerParameter != null) {
                    if (!instance.BubblerParameter.Compare(entity.BubblerParameter)) {
                        instance.BubblerParameter.Set(entity.BubblerParameter);
                        this._context.Entry<BubblerParameter>(instance.BubblerParameter).State = EntityState.Modified;
                    }
                }

                var updated = (await Task.Run(()=>this._context.PartInstances.Update(instance))).Entity;

                if (save) {
                    if (this.SaveChanges()) {
                        return updated;
                    } else {
                        return null;
                    }
                } else {
                    return updated;
                }
            } else {
                return null;
            }
        }

        public async Task<PartInstance> AddAsync(PartInstance entity, bool save) {
            var instance = await this._context.PartInstances.AsNoTracking().FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (instance != null) {
                return null;
            }

            if (entity.Part != null && entity.CurrentLocation != null && entity.Price != null) {

                if (entity.PartType != null) {
                    this._context.Entry<PartType>(entity.PartType).State = EntityState.Modified;
                }

                if (entity.Condition != null) {
                    this._context.Entry<Condition>(entity.Condition).State = EntityState.Modified;
                }

                if (entity.BubblerParameter != null) {
                    this._context.Entry<BubblerParameter>(entity.BubblerParameter).State = EntityState.Modified;
                }
                this._context.Entry<Part>(entity.Part).State = EntityState.Modified;
                this._context.Entry<Location>(entity.CurrentLocation).State = EntityState.Modified;
                var added=(await this._context.PartInstances.AddAsync(entity)).Entity;
                if (save) {
                    if (await this.SaveChangesAsync()) {
                        return added;
                    } else {
                        return null;
                    }
                } else {
                    return added;
                }
            } else {
                return null;
            }
        }

        public PartInstance GetEntity(Expression<Func<PartInstance, bool>> expression,bool tracking) {
            if (tracking) {
                return this._context.Set<PartInstance>()
                    .Include(e => e.Transactions)
                        .ThenInclude(e => e.Session)
                    .Include(e => e.Transactions)
                        .ThenInclude(e => e.Location)
                    .Include(e => e.PartType)
                    .Include(e => e.CurrentLocation)
                    .Include(e => e.Price)
                    .Include(e => e.BubblerParameter)
                    .Include(e => e.Condition)
                    .Include(e=>e.Part)
                    .FirstOrDefault(expression);
            } else {
                return this._context.Set<PartInstance>()
                    .AsNoTracking()
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
                    .FirstOrDefault(expression);
            }

        }
        
        public async Task<PartInstance> GetEntityAsync(Expression<Func<PartInstance, bool>> expression,bool tracking) {
            if (tracking) {
                return await this._context.Set<PartInstance>()
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
                    .FirstOrDefaultAsync(expression);
            } else {
                return await this._context.Set<PartInstance>()
                    .AsNoTracking()
                    .Include(e => e.Transactions)
                        .ThenInclude(e => e.Session)
                    .Include(e => e.PartType)
                    .Include(e => e.CurrentLocation)
                    .Include(e => e.Price)
                    .Include(e => e.BubblerParameter)
                    .Include(e => e.Condition)
                    .Include(e => e.Part)
                    .FirstOrDefaultAsync(expression);
            }
        }
        
        public IEnumerable<PartInstance> GetEntityList(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) {
            IQueryable<PartInstance> query = this._context.Set<PartInstance>()
                .AsNoTracking()
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Location)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                .Include(e => e.Condition);

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return  query.ToList();
            }
        }
        
        public async Task<IEnumerable<PartInstance>> GetEntityListAsync(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) {
            IQueryable<PartInstance> query = this._context.Set<PartInstance>()
                .AsNoTracking()
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Location)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                .Include(e => e.Condition);

            if (expression != null) {
                query=query.Where(expression);
            }

            if (orderBy != null) {
                return await orderBy(query).ToListAsync();
            } else {
                return await query.ToListAsync();
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
                .LoadAsync();
        }
        
        public async Task<bool> SaveChangesAsync(bool undoIfFail) {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    await this._context.SaveChangesAsync();
                }
                return true;
            } catch {
                if(undoIfFail)
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
