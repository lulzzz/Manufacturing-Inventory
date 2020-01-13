using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;
using ManufacturingInventory.Common.Model.DbContextExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace ManufacturingInventory.Common.Data {
    public class PartService : IEntityService<Part> {

        private ManufacturingContext _context;

        public PartService(ManufacturingContext context) {
            this._context = context;
        }

        public Part Add(Part entity, bool save) {
            var part = this.GetEntity(e => e.Id == entity.Id, true);
            if (part != null) {
                return null;
            }

            var added = this._context.Parts.Add(part).Entity;
            if (save) {
                if (this.SaveChanges()) {
                    return added;
                } else {
                    return null;
                }
            } else {
                return added;
            }
        }
        
        public async  Task<Part> AddAsync(Part entity, bool save) {
            var part = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (part != null) {
                return null;
            }
            
            var added = (await this._context.Parts.AddAsync(part)).Entity;
            if (save) {
                if (await this.SaveChangesAsync()) {
                    return added;
                } else {
                    return null;
                }
            } else {
                return added;
            }
        }
        
        public Part Update(Part entity, bool save) {
            var part = this.GetEntity(e => e.Id == entity.Id, true);
            if (part == null) {
                return null;
            }
            part.Set(entity);
            var updated = this._context.Parts.Update(part).Entity;
            if (save) {
                if (this.SaveChanges()) {
                    return updated;
                } else {
                    return null;
                }
            } else {
                return updated;
            }
        }
        
        public async Task<Part> UpdateAsync(Part entity, bool save) {
            var part = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (part == null) {
                return null;
            }
            part.Set(entity);
            var updated = (await Task.Run(() => this._context.Parts.Update(part))).Entity;
            if (save) {
                if(await this.SaveChangesAsync()) {
                    return updated;
                } else {
                    return null;
                }
            } else {
                return updated;
            }
        }

        public Part Delete(Part entity, bool save) => throw new NotImplementedException();
        public Task<Part> DeleteAsync(Part entity, bool save) => throw new NotImplementedException();

        public Part GetEntity(Expression<Func<Part, bool>> expression, bool tracking) {
            if (tracking) {
                return this._context.Parts
                    .AsNoTracking()
                    .Include(e => e.Organization)
                    .Include(e => e.Warehouse)
                    .Include(e => e.Usage)
                    .Include(e=>e.Attachments)
                    .FirstOrDefault(expression);
            } else {
                return this._context.Parts
                    .Include(e => e.Organization)
                    .Include(e => e.Warehouse)
                    .Include(e => e.Usage)
                    .Include(e => e.Attachments)
                    .FirstOrDefault(expression);
            }
        }
        public async Task<Part> GetEntityAsync(Expression<Func<Part, bool>> expression, bool tracking) {
            if (tracking) {
                return await this._context.Parts
                    .AsNoTracking()
                    .Include(e => e.Organization)
                    .Include(e => e.Warehouse)
                    .Include(e => e.Usage)
                    .Include(e => e.Attachments)
                    .FirstOrDefaultAsync(expression);
            } else {
                return await this._context.Parts
                    .Include(e => e.Organization)
                    .Include(e => e.Warehouse)
                    .Include(e => e.Usage)
                    .Include(e => e.Attachments)
                    .FirstOrDefaultAsync(expression);
            }
        }


        public IEnumerable<Part> GetEntityList(Expression<Func<Part, bool>> expression = null, Func<IQueryable<Part>, IOrderedQueryable<Part>> orderBy = null) {
            IQueryable<Part> query = this._context.Set<Part>()
                .AsNoTracking()
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .Include(e => e.Attachments);

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }
        
        public async Task<IEnumerable<Part>> GetEntityListAsync(Expression<Func<Part, bool>> expression = null, Func<IQueryable<Part>, IOrderedQueryable<Part>> orderBy = null) {
            IQueryable<Part> query=this._context.Set<Part>()
                .AsNoTracking()
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .Include(e => e.Attachments);

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return await orderBy(query).ToListAsync();
            } else {
                return await query.ToListAsync();
            }
        }

        public void Load() {
            this._context.Parts
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .Include(e => e.Attachments)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Parts
                .Include(e => e.Organization)
                .Include(e => e.Warehouse)
                .Include(e => e.Usage)
                .Include(e => e.Attachments)
                .LoadAsync();
        }

        public async Task<bool> SaveChangesAsync(bool undoIfFail) {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    await this._context.SaveChangesAsync();
                }
                return true;
            } catch {
                if (undoIfFail)
                    this._context.UndoDbContext();

                return false;
            }
        }

        public bool SaveChanges(bool undoIfFail) {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    this._context.SaveChanges();
                }
                return true;
            } catch {
                if (undoIfFail)
                    this._context.UndoDbContext();

                return false;
            }
        }

        public async Task<bool> SaveChangesAsync() {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    await this._context.SaveChangesAsync();
                }
                return true;
            } catch {
                return false;
            }
        }

        public bool SaveChanges() {
            try {
                if (this._context.ChangeTracker.HasChanges()) {
                    this._context.SaveChanges();
                }
                return true;
            } catch {
                return false;
            }
        }

    }
}
