using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class UserRepository : IRepository<User> {
        private ManufacturingContext _context;

        public UserRepository(ManufacturingContext context) {
            this._context = context;
        }

        public User Add(User entity) {
            return this._context.Add(entity).Entity;
        }

        public async Task<User> AddAsync(User entity) {
            return (await this._context.AddAsync(entity)).Entity;
        }

        public User Update(User entity) {
            var user = this.GetEntity(e => e.Id == entity.Id);
            if (user == null) {
                return null;
            }
            user.Set(entity);
            return this._context.Users.Update(user).Entity;
        }

        public async Task<User> UpdateAsync(User entity) {
            var user = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (user == null) {
                return null;
            }
            user.Set(entity);
            return this._context.Users.Update(user).Entity;
        }


        public User Delete(User entity) => throw new NotImplementedException();
        public Task<User> DeleteAsync(User entity) => throw new NotImplementedException();

        public User GetEntity(Expression<Func<User, bool>> expression) {
            return this._context.Users
                .Include(e => e.Sessions)
                .Include(e=>e.UserAlerts)
                .Include(e=>e.Permission)
                .FirstOrDefault(expression);
        }

        public async Task<User> GetEntityAsync(Expression<Func<User, bool>> expression) {
            return await this._context.Users
                .Include(e => e.Sessions)
                .Include(e => e.UserAlerts)
                .Include(e => e.Permission)
                .FirstOrDefaultAsync(expression);
        }


        public IEnumerable<User> GetEntityList(Expression<Func<User, bool>> expression = null, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null) {
            IQueryable<User> query = this._context.Set<User>()
                .Include(e => e.Sessions)
                .Include(e => e.UserAlerts)
                .Include(e => e.Permission)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return query.ToList();
            } else {
                return orderBy(query).ToList();
            }
        }

        public async Task<IEnumerable<User>> GetEntityListAsync(Expression<Func<User, bool>> expression = null, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null) {
            IQueryable<User> query = this._context.Set<User>()
                .Include(e => e.Sessions)
                .Include(e => e.UserAlerts)
                .Include(e => e.Permission)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return await query.ToListAsync();
            } else {
                return await orderBy(query).ToListAsync();
            }
        }

        public void Load() {
            this._context.Users
                .Include(e => e.Sessions)
                .Include(e => e.UserAlerts)
                .Include(e => e.Permission)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Users
                .Include(e => e.Sessions)
                .Include(e => e.UserAlerts)
                .Include(e => e.Permission)
                .LoadAsync();
        }
    }
}
