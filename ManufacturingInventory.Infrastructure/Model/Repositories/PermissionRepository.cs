using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class PermissionRepository : IRepository<Permission> {
        private ManufacturingContext _context;

        public PermissionRepository(ManufacturingContext context) {
            this._context = context;
        }

        public Permission Add(Permission entity) {
            return this._context.Add(entity).Entity;
        }

        public async Task<Permission> AddAsync(Permission entity) {
            return (await this._context.AddAsync(entity)).Entity;
        }

        public Permission Update(Permission entity) {
            var Permission = this.GetEntity(e => e.Id == entity.Id);
            if (Permission == null) {
                return null;
            }
            Permission.Set(entity);
            return this._context.Permissions.Update(Permission).Entity;
        }

        public async Task<Permission> UpdateAsync(Permission entity) {
            var Permission = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (Permission == null) {
                return null;
            }
            Permission.Set(entity);
            return this._context.Permissions.Update(Permission).Entity;
        }


        public Permission Delete(Permission entity) => throw new NotImplementedException();
        public Task<Permission> DeleteAsync(Permission entity) => throw new NotImplementedException();

        public Permission GetEntity(Expression<Func<Permission, bool>> expression) {
            return this._context.Permissions
                .Include(e => e.Users)
                .FirstOrDefault(expression);
        }

        public async Task<Permission> GetEntityAsync(Expression<Func<Permission, bool>> expression) {
            return await this._context.Permissions
                .Include(e => e.Users)
                .FirstOrDefaultAsync(expression);
        }


        public IEnumerable<Permission> GetEntityList(Expression<Func<Permission, bool>> expression = null, Func<IQueryable<Permission>, IOrderedQueryable<Permission>> orderBy = null) {
            IQueryable<Permission> query = this._context.Set<Permission>()
                .Include(e => e.Users)
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

        public async Task<IEnumerable<Permission>> GetEntityListAsync(Expression<Func<Permission, bool>> expression = null, Func<IQueryable<Permission>, IOrderedQueryable<Permission>> orderBy = null) {
            IQueryable<Permission> query = this._context.Set<Permission>()
                .Include(e => e.Users)
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
            this._context.Permissions
                .Include(e => e.Users)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Permissions
                .Include(e=>e.Users)
                .LoadAsync();
        }
    }
}
