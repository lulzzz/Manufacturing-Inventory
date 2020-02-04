using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class PartInstanceRepository : IRepository<PartInstance> {
        private ManufacturingContext _context;

        public PartInstanceRepository(ManufacturingContext context) {
            this._context = context;
        }

        public PartInstance Add(PartInstance entity) {
            var instance = this.GetEntity(e => e.Id == entity.Id);
            if (instance != null) {
                return null;
            }
            return this._context.Add(entity).Entity;
        }

        public PartInstance Delete(PartInstance entity) {
            var toRemove = this._context.PartInstances.FirstOrDefault(e => e.Id == entity.Id);
            if (toRemove != null) {

                var removed = this._context.PartInstances.Remove(toRemove).Entity;
                return removed;
            } else {
                return null;
            }
        }

        public async Task<PartInstance> DeleteAsync(PartInstance entity) {
            var toRemove = await this._context.PartInstances.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (toRemove != null) {
                var removed = (await Task.Run(() => {
                    return this._context.PartInstances.Remove(toRemove);
                })).Entity;

                return removed;
            } else {
                return null;
            }
        }

        public PartInstance Update(PartInstance entity) {
            var instance = this.GetEntity(e => e.Id == entity.Id);
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
                instance.SkuNumber = entity.SkuNumber;
                instance.ConditionId = entity.ConditionId;
                instance.LocationId = entity.LocationId;
                instance.PartTypeId = entity.PartTypeId;
                instance.PriceId = entity.PriceId;
                return this._context.PartInstances.Update(instance).Entity;
            } else {
                return null;
            }
        }

        public async Task<PartInstance> UpdateAsync(PartInstance entity) {
            var instance = await this.GetEntityAsync(e => e.Id == entity.Id);
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
                instance.SkuNumber = entity.SkuNumber;
                instance.ConditionId = entity.ConditionId;
                instance.LocationId = entity.LocationId;
                instance.PartTypeId = entity.PartTypeId;
                instance.PriceId = entity.PriceId;
                return this._context.PartInstances.Update(instance).Entity;
            } else {
                return null;
            }
        }

        public async Task<PartInstance> AddAsync(PartInstance entity) {
            var instance = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (instance != null) {
                return null;
            }
            return (await this._context.AddAsync(entity)).Entity;
        }

        public PartInstance GetEntity(Expression<Func<PartInstance, bool>> expression) {
            return this._context.PartInstances
                .Include(e => e.Price)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Condition)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                    .ThenInclude(e => e.PartPrices)
                .Include(e=>e.PriceLogs)
                .FirstOrDefault(expression);
        }

        public async Task<PartInstance> GetEntityAsync(Expression<Func<PartInstance, bool>> expression) {
            return await this._context.PartInstances
                .Include(e => e.Price)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Condition)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                    .ThenInclude(e => e.PartPrices)
                .Include(e => e.PriceLogs)
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<PartInstance> GetEntityList(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) {
            IQueryable<PartInstance> query = this._context.Set<PartInstance>()
                .Include(e => e.Price)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Condition)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                    .ThenInclude(e => e.PartPrices)
                .Include(e => e.PriceLogs)
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
                .Include(e => e.Price)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Condition)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                    .ThenInclude(e => e.PartPrices)
                .Include(e => e.PriceLogs)
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
                .Include(e => e.Price)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Condition)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.PartInstances
                .Include(e => e.Price)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Condition)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Part)
                    .ThenInclude(e=>e.PartPrices)
                .Include(e => e.PriceLogs)
                .LoadAsync();
        }
    }
}
