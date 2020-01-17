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

        public Attachment Add(Attachment entity) => throw new NotImplementedException();
        public Task<Attachment> AddAsync(Attachment entity) => throw new NotImplementedException();
        public Attachment Update(Attachment entity) => throw new NotImplementedException();
        public Task<Attachment> UpdateAsync(Attachment entity) => throw new NotImplementedException();
        public Attachment Delete(Attachment entity) => throw new NotImplementedException();
        public Task<Attachment> DeleteAsync(Attachment entity) => throw new NotImplementedException();

        public Attachment GetEntity(Expression<Func<Attachment, bool>> expression) {
            return this._context.Attachments.FirstOrDefault(expression);
        }

        public async Task<Attachment> GetEntityAsync(Expression<Func<Attachment, bool>> expression) {
            return await this._context.Attachments.FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Attachment> GetEntityList(Expression<Func<Attachment, bool>> expression = null, 
            Func<IQueryable<Attachment>, IOrderedQueryable<Attachment>> orderBy = null) {
            
            IQueryable<Attachment> query = this._context.Set<Attachment>()
                .Include(e => e.PartInstance).Include(e => e.Price)
                .Include(e => e.Distributor).Include(e => e.Part).AsNoTracking();
            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return query.ToList();
            } else {
                return orderBy(query).ToList();
            }
        }

        public async Task<IEnumerable<Attachment>> GetEntityListAsync(Expression<Func<Attachment, bool>> expression = null, 
            Func<IQueryable<Attachment>, IOrderedQueryable<Attachment>> orderBy = null) {
            IQueryable<Attachment> query = this._context.Set<Attachment>()
                .Include(e => e.PartInstance)
                .Include(e => e.Price)
                .Include(e => e.Distributor)
                .Include(e => e.Part)
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
            this._context.Attachments
                .Include(e => e.PartInstance)
                .Include(e => e.Price)
                .Include(e => e.Distributor)
                .Include(e => e.Part)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Attachments
                .Include(e => e.PartInstance)
                .Include(e => e.Price)
                .Include(e => e.Distributor)
                .Include(e => e.Part)
                .LoadAsync();
        }

    }
}
