using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class BubblerParameterRepository : IRepository<BubblerParameter> {
        private ManufacturingContext _context;

        public BubblerParameterRepository(ManufacturingContext context) {
            this._context = context;
        }

        public BubblerParameter Add(BubblerParameter entity) => throw new NotImplementedException();
        public Task<BubblerParameter> AddAsync(BubblerParameter entity) => throw new NotImplementedException();
        
        public BubblerParameter Update(BubblerParameter entity) {
            var param = this.GetEntity(e => e.Id == entity.Id);
            if (param == null) {
                return null;
            }

            param.Set(entity);
            return this._context.Update(param).Entity;
        }

        public async Task<BubblerParameter> UpdateAsync(BubblerParameter entity) {
            var param =await this.GetEntityAsync(e => e.Id == entity.Id);
            if (param == null) {
                return null;
            }

            param.Set(entity);
            return this._context.Update(param).Entity;
        }
       
        public BubblerParameter Delete(BubblerParameter entity) => throw new NotImplementedException();
        public Task<BubblerParameter> DeleteAsync(BubblerParameter entity) => throw new NotImplementedException();


        public BubblerParameter GetEntity(Expression<Func<BubblerParameter, bool>> expression) {
            return this._context.BubblerParameters.Include(e => e.PartInstance).FirstOrDefault(expression);
        }

        public async Task<BubblerParameter> GetEntityAsync(Expression<Func<BubblerParameter, bool>> expression) {
            return await this._context.BubblerParameters.Include(e => e.PartInstance).FirstOrDefaultAsync(expression);
        }


        public IEnumerable<BubblerParameter> GetEntityList(Expression<Func<BubblerParameter, bool>> expression = null, Func<IQueryable<BubblerParameter>, IOrderedQueryable<BubblerParameter>> orderBy = null) {
            IQueryable<BubblerParameter> query = this._context.Set<BubblerParameter>().Include(e => e.PartInstance).AsNoTracking();
            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return query.ToList();
            } else {
                return orderBy(query).ToList();
            }
        }

        public async Task<IEnumerable<BubblerParameter>> GetEntityListAsync(Expression<Func<BubblerParameter, bool>> expression = null, Func<IQueryable<BubblerParameter>, IOrderedQueryable<BubblerParameter>> orderBy = null) {
            IQueryable<BubblerParameter> query = this._context.Set<BubblerParameter>().Include(e => e.PartInstance).AsNoTracking();
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
            this._context.BubblerParameters.Include(e => e.PartInstance).Load();
        }
        
        public async Task LoadAsync() {
            await this._context.BubblerParameters.Include(e => e.PartInstance).LoadAsync();
        }

    }
}
