using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class AlertRepository : IRepository<Alert> {
        private ManufacturingContext _context;

        public AlertRepository(ManufacturingContext context) {
            this._context = context;
        }

        public Alert Add(Alert entity) {
            var alert = this.GetEntity(e => e.Id == entity.Id);
            if (alert != null) {
                return null;
            }
            return this._context.Alerts.Add(entity).Entity;
        }

        public Alert Update(Alert entity) {
            var alert = this.GetEntity(e => e.Id == entity.Id);
            if (alert == null) {
                return null;
            }
            alert.UserAlerts = entity.UserAlerts;
            switch (alert.AlertType) {
                case AlertType.IndividualAlert:
                    ((IndividualAlert)alert).PartInstance = ((IndividualAlert)entity).PartInstance;
                    return this._context.Update<Alert>(alert).Entity;
                case AlertType.CombinedAlert:
                    ((CombinedAlert)alert).StockHolder = ((CombinedAlert)entity).StockHolder;
                    return this._context.Update<Alert>(alert).Entity;
                default:
                    return null;
            }
        }

        public async Task<Alert> UpdateAsync(Alert entity) {
            var alert = this.GetEntity(e => e.Id == entity.Id);
            if (alert == null) {
                return null;
            }
            alert.UserAlerts = entity.UserAlerts;
            switch (alert.AlertType) {
                case AlertType.IndividualAlert:
                    ((IndividualAlert)alert).PartInstance = ((IndividualAlert)entity).PartInstance;
                    return (await Task.Run(() => this._context.Update<Alert>(alert))).Entity;
                case AlertType.CombinedAlert:
                    ((CombinedAlert)alert).StockHolder = ((CombinedAlert)entity).StockHolder;
                    return (await Task.Run(() => this._context.Update<Alert>(alert))).Entity;
                default:
                    return null;
            }
            
        }

        public async Task<Alert> AddAsync(Alert entity) {
            var alert = await this._context.Alerts.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (alert != null) {
                return null;
            }
            return (await this._context.Alerts.AddAsync(entity)).Entity;
        }

        public Alert Delete(Alert entity) {
            var alert = this._context.Alerts.Include(e=>e.UserAlerts).FirstOrDefault(e => e.Id == entity.Id);
            if (alert == null) {
                return null;
            }
            this._context.RemoveRange(alert.UserAlerts);
            return this._context.Alerts.Remove(alert).Entity;
        }

        public async Task<Alert> DeleteAsync(Alert entity) {
            var alert = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (alert == null) {
                return null;
            }
            this._context.RemoveRange(alert.UserAlerts);
            return (await Task.Run(() => this._context.Alerts.Remove(alert))).Entity;
        }

        public Alert GetEntity(Expression<Func<Alert, bool>> expression) {
            return this._context.Alerts
                .Include(e=>e.UserAlerts)
                .FirstOrDefault(expression);
        }

        public async Task<Alert> GetEntityAsync(Expression<Func<Alert, bool>> expression) {
            return await this._context.Alerts
                .Include(e=>e.UserAlerts)
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Alert> GetEntityList(Expression<Func<Alert, bool>> expression = null, Func<IQueryable<Alert>, IOrderedQueryable<Alert>> orderBy = null) {
            IQueryable<Alert> query = this._context.Set<Alert>()
                .Include(e => e.UserAlerts)
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

        public async Task<IEnumerable<Alert>> GetEntityListAsync(Expression<Func<Alert, bool>> expression = null, Func<IQueryable<Alert>, IOrderedQueryable<Alert>> orderBy = null) {
            IQueryable<Alert> query = this._context.Set<Alert>()
                .Include(e => e.UserAlerts)
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
            this._context.Alerts
                .Include(e => e.UserAlerts)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Alerts
                .Include(e => e.UserAlerts)
                .LoadAsync();
        }
    }
}
