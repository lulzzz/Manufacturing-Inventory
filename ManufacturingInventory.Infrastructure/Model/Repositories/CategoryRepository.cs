using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class CategoryRepository:IRepository<Category> {
        private ManufacturingContext _context;

        public CategoryRepository(ManufacturingContext context) {
            this._context = context;
        }

        public Category Add(Category entity) {
            var category = this.GetEntity(e => e.Id == entity.Id);
            if (category != null) {
                return null;
            }
            return this._context.Categories.Add(entity).Entity;
        }

        public Category Update(Category entity) {
            var category = this.GetEntity(e => e.Id == entity.Id);
            if (category == null) {
                return null;
            }
            category.Set(entity);
            return  this._context.Update<Category>(category).Entity;
        }

        public async Task<Category> UpdateAsync(Category entity) {
            var category = this.GetEntity(e => e.Id == entity.Id);
            if (category == null) {
                return null;
            }
            category.Set(entity);
            return (await Task.Run(() => this._context.Update<Category>(category))).Entity;
        }

        public async Task<Category> AddAsync(Category entity) {
            var category = await this._context.Categories.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (category != null) {
                return null;
            }

            return (await this._context.Categories.AddAsync(entity)).Entity;
        }

        public Category Delete(Category entity) {
            var category = this._context.Categories.FirstOrDefault(e => e.Id == entity.Id);
            if (category == null) {
                return null;
            }
            return this._context.Categories.Remove(category).Entity;
        }

        public async Task<Category> DeleteAsync(Category entity) {
            var category = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (category == null) {
                return null;
            }
            return (await Task.Run(() => this._context.Categories.Remove(category))).Entity;
        }

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
