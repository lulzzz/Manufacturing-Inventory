using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class SessionRepository : IRepository<Session> {
        private ManufacturingContext _context;

        public SessionRepository(ManufacturingContext context) {
            this._context = context;
        }

        public Session Add(Session entity) {
            return this._context.Add(entity).Entity;
        }

        public async Task<Session> AddAsync(Session entity) {
            return (await this._context.AddAsync(entity)).Entity;
        }

        public Session Update(Session entity) {
            var Session = this.GetEntity(e => e.Id == entity.Id);
            if (Session == null) {
                return null;
            }
            Session.Set(entity);
            return this._context.Sessions.Update(Session).Entity;
        }

        public async Task<Session> UpdateAsync(Session entity) {
            var Session = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (Session == null) {
                return null;
            }
            Session.Set(entity);
            return this._context.Sessions.Update(Session).Entity;
        }


        public Session Delete(Session entity) => throw new NotImplementedException();
        public Task<Session> DeleteAsync(Session entity) => throw new NotImplementedException();

        public Session GetEntity(Expression<Func<Session, bool>> expression) {
            return this._context.Sessions
                .Include(e => e.User)
                .Include(e => e.Transactions)
                .FirstOrDefault(expression);
        }

        public async Task<Session> GetEntityAsync(Expression<Func<Session, bool>> expression) {
            return await this._context.Sessions
                .Include(e => e.User)
                .Include(e => e.Transactions)
                .FirstOrDefaultAsync(expression);
        }


        public IEnumerable<Session> GetEntityList(Expression<Func<Session, bool>> expression = null, Func<IQueryable<Session>, IOrderedQueryable<Session>> orderBy = null) {
            IQueryable<Session> query = this._context.Set<Session>()
                .Include(e => e.User)
                .Include(e => e.Transactions)
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

        public async Task<IEnumerable<Session>> GetEntityListAsync(Expression<Func<Session, bool>> expression = null, Func<IQueryable<Session>, IOrderedQueryable<Session>> orderBy = null) {
            IQueryable<Session> query = this._context.Set<Session>()
                .Include(e => e.User)
                .Include(e => e.Transactions)
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
            this._context.Sessions
                .Include(e => e.User)
                .Include(e => e.Transactions)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Sessions
                .Include(e=>e.User)
                .Include(e=>e.Transactions)
                .LoadAsync();
        }
    }
}
