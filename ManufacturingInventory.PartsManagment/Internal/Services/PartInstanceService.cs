using ManufacturingInventory.Common.Buisness.Interfaces;
using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.PartsManagment.Internal.Services {
    public class PartInstanceService : IEntityService<PartInstance> {
        private ManufacturingContext _context;
        private IUserService _userService;

        public PartInstanceService(ManufacturingContext context, IUserService userService) {
            this._userService = userService;
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
            return null;
        }

        public PartInstance Update(PartInstance entity, bool save) {
            var instance = this._context.PartInstances.AsNoTracking().FirstOrDefault(e => e.Id == entity.Id);
            if (instance == null) {
                return null;
            }

            this._context.PartInstances.Update(entity);

            if (save) {
                if (this.SaveChanges()) {
                    return entity;
                } else {
                    return null;
                }
            } else {
                return entity;
            }
        }

        public Task<PartInstance> UpdateAsync(PartInstance entity, bool save) {
            return null;
        }

        public Task<PartInstance> AddAsync(PartInstance entity, bool save) {
            return null;
        }

        public Task<PartInstance> DeleteAsync(PartInstance entity,bool save) {
            return null;
        }

        public PartInstance GetPartInstance(Expression<Func<PartInstance, bool>> expression) {
            return this._context.Set<PartInstance>().AsNoTracking()
                .AsNoTracking()
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .FirstOrDefault(expression);
        }
        
        public async Task<PartInstance> GetPartInstanceAsync(Expression<Func<PartInstance, bool>> expression) {
            return await this._context.Set<PartInstance>().AsNoTracking()
                    .AsNoTracking()
                    .Include(e => e.Transactions)
                        .ThenInclude(e => e.Session)
                    .Include(e => e.PartType)
                    .Include(e => e.CurrentLocation)
                    .Include(e => e.Price)
                    .Include(e => e.BubblerParameter)
                    .Include(e => e.Condition)
                    .FirstOrDefaultAsync(expression);
        }
        
        public IEnumerable<PartInstance> GetPartInstanceList(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) {
            IQueryable<PartInstance> query = this._context.Set<PartInstance>()
                .AsNoTracking()
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition);

            if (expression != null) {
                query.Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return  query.ToList();
            }
        }
        
        public async Task<IEnumerable<PartInstance>> GetPartInstanceListAsync(Expression<Func<PartInstance, bool>> expression = null, Func<IQueryable<PartInstance>, IOrderedQueryable<PartInstance>> orderBy = null) {
            IQueryable<PartInstance> query = this._context.Set<PartInstance>()
                .AsNoTracking()
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition);

            if (expression != null) {
                query.Where(expression);
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
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .Load();
        }
        
        public async Task LoadAsync() {
            await this._context.PartInstances
                .Include(e => e.Transactions)
                    .ThenInclude(e => e.Session)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .LoadAsync();
        }

        public async Task<bool> SaveChangesAsync() {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    await this.SaveChangesAsync();
                }
                return true;
            } catch {
                return false;
            }

        }

        public bool SaveChanges() {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    this.SaveChanges();
                }
                return true;
            } catch {
                return false;
            }
        }
    }
}
