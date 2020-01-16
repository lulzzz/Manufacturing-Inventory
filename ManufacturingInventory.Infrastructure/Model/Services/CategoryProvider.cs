using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Services {
    public class CategoryProvider : IEntityProvider<Category> {
        private ManufacturingContext _context;

        public CategoryProvider(ManufacturingContext context) => this._context = context;

        public Category GetEntity(Expression<Func<Category, bool>> expression) {
            return this._context.Categories
                .FirstOrDefault(expression);
        }

        public async Task<Category> GetEntityAsync(Expression<Func<Category, bool>> expression) {
            return await this._context.Categories
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Category> GetEntityList(Expression<Func<Category, bool>> expression = null, Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null) {
            IQueryable<Category> query = this._context.Set<Category>()
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

        public async Task<IEnumerable<Category>> GetEntityListAsync(Expression<Func<Category, bool>> expression = null, Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null) {
            IQueryable<Category> query = this._context.Set<Category>().AsNoTracking();

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
            this._context.Categories.Load();
        }

        public async Task LoadAsync() {
            await this._context.Categories.LoadAsync();
        }
    }
}
