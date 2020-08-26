using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {

    //public class UserUserAlertRepository : IRepository<UserAlert> {
    //    private ManufacturingContext _context;

    //    public UserAlertRepository(ManufacturingContext context) {
    //        this._context = context;
    //    }

    //    public UserAlert Add(UserAlert entity) {
    //        var userAlert = this.GetEntity(e => e.Id == entity.Id);
    //        if (userAlert != null) {
    //            return null;
    //        }
    //        return this._context.UserAlerts.Add(entity).Entity;
    //    }

    //    public UserAlert Update(UserAlert entity) {
    //        //var userAlert = this.GetEntity(e => e.Id == entity.Id);
    //        //if (userAlert == null) {
    //        //    return null;
    //        //}
    //        return null;
    //    }

    //    public async Task<UserAlert> UpdateAsync(UserAlert entity) {
    //        //var userAlert = this.GetEntity(e => e.Id == entity.Id);
    //        //if (userAlert == null) {
    //        //    return null;
    //        //}

    //        return null;
    //    }

    //    public async Task<UserAlert> AddAsync(UserAlert entity) {
    //        var userAlert = await this._context.UserAlerts.FirstOrDefaultAsync(e => e.Id == entity.Id);
    //        if (userAlert != null) {
    //            return null;
    //        }
    //        return (await this._context.UserAlerts.AddAsync(entity)).Entity;
    //    }

    //    public UserAlert Delete(UserAlert entity) {
    //        var userAlert = this._context.UserAlerts
    //            .Include(e => e.Alert)
    //            .Include(e => e.User)
    //            .FirstOrDefault(e => e.UserId == entity.UserId && e.AlertId == entity.AlertId);

    //        if (userAlert == null) {
    //            return null;
    //        }
    //        this._context.RemoveRange(userAlert.UserUserAlerts);
    //        return this._context.UserAlerts.Remove(userAlert).Entity;
    //    }

    //    public async Task<UserAlert> DeleteAsync(UserAlert entity) {
    //        var userAlert = await this.GetEntityAsync(e => e.Id == entity.Id);
    //        if (userAlert == null) {
    //            return null;
    //        }
    //        this._context.RemoveRange(userAlert.UserUserAlerts);
    //        return (await Task.Run(() => this._context.UserAlerts.Remove(userAlert))).Entity;
    //    }

    //    public UserAlert GetEntity(Expression<Func<UserAlert, bool>> expression) {
    //        return this._context.UserAlerts
    //            .Include(e => e.UserUserAlerts)
    //            .FirstOrDefault(expression);
    //    }

    //    public async Task<UserAlert> GetEntityAsync(Expression<Func<UserAlert, bool>> expression) {
    //        return await this._context.UserAlerts
    //            .Include(e => e.UserUserAlerts)
    //            .FirstOrDefaultAsync(expression);
    //    }

    //    public IEnumerable<UserAlert> GetEntityList(Expression<Func<UserAlert, bool>> expression = null, Func<IQueryable<UserAlert>, IOrderedQueryable<UserAlert>> orderBy = null) {
    //        IQueryable<UserAlert> query = this._context.Set<UserAlert>()
    //            .Include(e => e.UserUserAlerts)
    //            .AsNoTracking();

    //        if (expression != null) {
    //            query = query.Where(expression);
    //        }

    //        if (orderBy != null) {
    //            return orderBy(query).ToList();
    //        } else {
    //            return query.ToList();
    //        }
    //    }

    //    public async Task<IEnumerable<UserAlert>> GetEntityListAsync(Expression<Func<UserAlert, bool>> expression = null, Func<IQueryable<UserAlert>, IOrderedQueryable<UserAlert>> orderBy = null) {
    //        IQueryable<UserAlert> query = this._context.Set<UserAlert>()
    //            .Include(e => e.UserUserAlerts)
    //            .AsNoTracking();

    //        if (expression != null) {
    //            query = query.Where(expression).AsNoTracking();
    //        }

    //        if (orderBy != null) {
    //            return await orderBy(query).ToListAsync();
    //        } else {
    //            return await query.ToListAsync();
    //        }
    //    }

    //    public void Load() {
    //        this._context.UserAlerts
    //            .Include(e => e.UserUserAlerts)
    //            .Load();
    //    }

    //    public async Task LoadAsync() {
    //        await this._context.UserAlerts
    //            .Include(e => e.UserUserAlerts)
    //            .LoadAsync();
    //    }
    //}
}
