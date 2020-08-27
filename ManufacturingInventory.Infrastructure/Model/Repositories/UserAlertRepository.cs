using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {

    public class UserAlertRepository : IRepository<UserAlert> {
        private ManufacturingContext _context;

        public UserAlertRepository(ManufacturingContext context) {
            this._context = context;
        }

        public UserAlert Add(UserAlert entity) {
            var userAlert = this._context.UserAlerts.FirstOrDefault(e => e.AlertId == entity.AlertId && e.UserId == entity.UserId);
            if (userAlert != null) {
                return null;
            }
            return this._context.UserAlerts.Add(entity).Entity;
        }

        public async Task<UserAlert> AddAsync(UserAlert entity) {
            var userAlert = await this._context.UserAlerts.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (userAlert != null) {
                return null;
            }
            return (await this._context.UserAlerts.AddAsync(entity)).Entity;
        }

        public UserAlert Update(UserAlert entity) {
            var userAlert = this._context.UserAlerts.FirstOrDefault(e => e.AlertId == entity.AlertId && e.UserId == entity.UserId);
            if (userAlert == null) {
                return null;
            }
            userAlert.IsEnabled = entity.IsEnabled;
            return this._context.UserAlerts.Update(userAlert).Entity;
        }

        public async Task<UserAlert> UpdateAsync(UserAlert entity) {
            var userAlert = await this._context.UserAlerts.FirstOrDefaultAsync(e => e.AlertId == entity.AlertId && e.UserId == entity.UserId);
            if (userAlert == null) {
                return null;
            }
            userAlert.IsEnabled = entity.IsEnabled;
            return this._context.UserAlerts.Update(userAlert).Entity;
        }

        public UserAlert Delete(UserAlert entity) {
            var userAlert= this._context.UserAlerts.FirstOrDefault(e=>e.AlertId==entity.AlertId && e.UserId==entity.UserId);
            if (userAlert == null) {
                return null;
            }

            return this._context.UserAlerts.Remove(userAlert).Entity;
        }

        public async Task<UserAlert> DeleteAsync(UserAlert entity) {
            var userAlert =await this._context.UserAlerts
                .FirstOrDefaultAsync(e => e.AlertId == entity.AlertId && e.UserId == entity.UserId);
            if (userAlert == null) {
                return null;
            }

            return this._context.UserAlerts.Remove(userAlert).Entity;
        }

        public UserAlert GetEntity(Expression<Func<UserAlert, bool>> expression) {
            return this._context.UserAlerts
                .Include(e => (e.Alert as IndividualAlert).PartInstance.BubblerParameter)
                .Include(e => (e.Alert as IndividualAlert).PartInstance.Part)
                .Include(e => (e.Alert as CombinedAlert).StockHolder.PartInstances)
                    .ThenInclude(e => e.Part)
                .FirstOrDefault(expression);
        }

        public async Task<UserAlert> GetEntityAsync(Expression<Func<UserAlert, bool>> expression) {
            return await this._context.UserAlerts
                    .Include(e => (e.Alert as IndividualAlert).PartInstance.BubblerParameter)
                    .Include(e => (e.Alert as IndividualAlert).PartInstance.Part)
                    .Include(e => (e.Alert as CombinedAlert).StockHolder.PartInstances)
                        .ThenInclude(e => e.Part)
                    .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<UserAlert> GetEntityList(Expression<Func<UserAlert, bool>> expression = null, Func<IQueryable<UserAlert>, IOrderedQueryable<UserAlert>> orderBy = null) {
            IQueryable<UserAlert> query = this._context.Set<UserAlert>()
                .Include(e => (e.Alert as IndividualAlert).PartInstance.BubblerParameter)
                .Include(e => (e.Alert as IndividualAlert).PartInstance.Part)
                .Include(e => (e.Alert as CombinedAlert).StockHolder.PartInstances)
                    .ThenInclude(e => e.Part)
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

        public async Task<IEnumerable<UserAlert>> GetEntityListAsync(Expression<Func<UserAlert, bool>> expression = null, Func<IQueryable<UserAlert>, IOrderedQueryable<UserAlert>> orderBy = null) {
            IQueryable<UserAlert> query = this._context.Set<UserAlert>()
                .Include(e => (e.Alert as IndividualAlert).PartInstance.BubblerParameter)
                .Include(e => (e.Alert as IndividualAlert).PartInstance.Part)
                .Include(e => (e.Alert as CombinedAlert).StockHolder.PartInstances)
                    .ThenInclude(e => e.Part)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression).AsNoTracking();
            }

            if (orderBy != null) {
                return await orderBy(query).ToListAsync();
            } else {
                return await query.ToListAsync();
            }
        }

        public void Load() {
            this._context.UserAlerts
            .Include(e => (e.Alert as IndividualAlert).PartInstance.BubblerParameter)
            .Include(e => (e.Alert as IndividualAlert).PartInstance.Part)
            .Include(e => (e.Alert as CombinedAlert).StockHolder.PartInstances)
                    .ThenInclude(e => e.Part)
            .Load();
        }

        public async Task LoadAsync() {
            await this._context.UserAlerts
                .Include(e => (e.Alert as IndividualAlert).PartInstance.BubblerParameter)
                .Include(e => (e.Alert as IndividualAlert).PartInstance.Part)
                .Include(e => (e.Alert as CombinedAlert).StockHolder.PartInstances)
                    .ThenInclude(e=>e.Part)
                .LoadAsync();
        }
    }
}
