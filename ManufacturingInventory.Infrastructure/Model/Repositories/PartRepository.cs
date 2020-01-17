using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class PartRepository : IRepository<Part> {
        private ManufacturingContext _context;

        public PartRepository(ManufacturingContext context) {
            this._context = context;
        }

        public Part Add(Part entity) {
            var part = this.GetEntity(e => e.Id == entity.Id);
            if (part != null) {
                return null;
            }

            return this._context.Parts.Add(part).Entity;
        }

        public async Task<Part> AddAsync(Part entity) {
            var part = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (part != null) {
                return null;
            }

            return (await this._context.Parts.AddAsync(part)).Entity;
        }

        public Part Update(Part entity) {
            var part = this.GetEntity(e => e.Id == entity.Id);
            if (part == null) {
                return null;
            }
            part.Set(entity);
            return this._context.Parts.Update(part).Entity;
        }

        public async Task<Part> UpdateAsync(Part entity) {
            var part = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (part == null) {
                return null;
            }
            part.Set(entity);
            return (await Task.Run(() => this._context.Parts.Update(part))).Entity;
        }

        public Part Delete(Part entity) => throw new NotImplementedException();
        
        public Task<Part> DeleteAsync(Part entity) => throw new NotImplementedException();

        public Part GetEntity(Expression<Func<Part, bool>> expression) {
                return this._context.Parts
                    .Include(e => e.Organization)
                    .Include(e => e.Warehouse)
                    .Include(e => e.Usage)
                    .Include(e => e.Attachments)
                    .AsNoTracking()
                    .FirstOrDefault(expression);
        }

        public async Task<Part> GetEntityAsync(Expression<Func<Part, bool>> expression) {
                return await this._context.Parts
                    .Include(e => e.Organization)
                    .Include(e => e.Warehouse)
                    .Include(e => e.Usage)
                    .Include(e => e.Attachments)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Part> GetEntityList(Expression<Func<Part, bool>> expression = null, Func<IQueryable<Part>, IOrderedQueryable<Part>> orderBy = null) {
            IQueryable<Part> query = this._context.Set<Part>()
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .Include(e => e.Attachments)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression).AsNoTracking();
            }

            if (orderBy != null) {
                return orderBy(query).AsNoTracking().ToList();
            } else {
                return query.AsNoTracking().ToList();
            }
        }

        public async Task<IEnumerable<Part>> GetEntityListAsync(Expression<Func<Part, bool>> expression = null, Func<IQueryable<Part>, IOrderedQueryable<Part>> orderBy = null) {
            IQueryable<Part> query = this._context.Set<Part>()
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .Include(e => e.Attachments)
                .AsNoTracking();

            if (expression != null) {
                query = query.Where(expression).AsNoTracking();
            }

            if (orderBy != null) {
                return await orderBy(query).AsNoTracking().ToListAsync();
            } else {
                return await query.AsNoTracking().ToListAsync();
            }
        }

        public void Load() {
            this._context.Parts
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .Include(e => e.Attachments)
                .AsNoTracking()
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Parts.AsNoTracking()
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .Include(e => e.Attachments)
                .AsNoTracking()
                .LoadAsync();
        }
    }
}
