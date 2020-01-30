using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class AttachmentRepository : IRepository<Attachment> {
        private ManufacturingContext _context;

        public AttachmentRepository(ManufacturingContext context) {
            this._context = context;
        }

        public Attachment Add(Attachment entity) {
            var attachment = this._context.Attachments.FirstOrDefault(e => e.Id == entity.Id);
            if (attachment != null) {
                return null;
            }
            return this._context.Add(entity).Entity;
        }

        public async Task<Attachment> AddAsync(Attachment entity) {
            var attachment = await this._context.Attachments.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (attachment != null) {
                return null;
            }
            return (await this._context.AddAsync(entity)).Entity;
        }

        public Attachment Update(Attachment entity) {
            var attachment = this._context.Attachments.FirstOrDefault(e => e.Id == entity.Id);
            if (attachment == null) {
                return null;
            }
            attachment.Set(entity);
            return this._context.Update(attachment).Entity;
        }

        public async Task<Attachment> UpdateAsync(Attachment entity) {
            var attachment = await this._context.Attachments.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (attachment == null) {
                return null;
            }
            attachment.Set(entity);
            return this._context.Update(attachment).Entity;
        }

        public Attachment Delete(Attachment entity) {
            return this._context.Attachments.Remove(entity).Entity;
        }

        public async Task<Attachment> DeleteAsync(Attachment entity) {
            var attachment= await Task.Run(() => this._context.Attachments.Remove(entity).Entity);
            return attachment;
        }

        public Attachment GetEntity(Expression<Func<Attachment, bool>> expression) {
            return this._context.Attachments.FirstOrDefault(expression);
        }

        public async Task<Attachment> GetEntityAsync(Expression<Func<Attachment, bool>> expression) {
            return await this._context.Attachments.FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Attachment> GetEntityList(Expression<Func<Attachment, bool>> expression = null, 
            Func<IQueryable<Attachment>, IOrderedQueryable<Attachment>> orderBy = null) {
            
            IQueryable<Attachment> query = this._context.Set<Attachment>()
                .Include(e => e.PartInstance)
                .Include(e => e.Price)
                .Include(e => e.Distributor)
                .Include(e => e.Part)
                .Include(e => e.Manufacturer)
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

        public async Task<IEnumerable<Attachment>> GetEntityListAsync(Expression<Func<Attachment, bool>> expression = null, 
            Func<IQueryable<Attachment>, IOrderedQueryable<Attachment>> orderBy = null) {
            IQueryable<Attachment> query = this._context.Set<Attachment>()
                .Include(e => e.PartInstance)
                .Include(e => e.Price)
                .Include(e => e.Distributor)
                .Include(e => e.Part)
                .Include(e => e.Manufacturer)
                .AsNoTracking();

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
            this._context.Attachments
                .Include(e => e.PartInstance)
                .Include(e => e.Price)
                .Include(e => e.Distributor)
                .Include(e => e.Part)
                .Include(e=>e.Manufacturer)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Attachments
                .Include(e => e.PartInstance)
                .Include(e => e.Price)
                .Include(e => e.Distributor)
                .Include(e => e.Part)
                .Include(e => e.Manufacturer)
                .LoadAsync();
        }

    }
}
