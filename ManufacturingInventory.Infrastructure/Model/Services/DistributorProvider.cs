using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Providers {
    public class DistributorProvider : IEntityProvider<Distributor> {
        private ManufacturingContext _context;

        public DistributorProvider(ManufacturingContext context) {
            this._context = context;
        }

        public Distributor GetEntity(Expression<Func<Distributor, bool>> expression) {
            return this._context.Distributors
                .Include(e => e.Attachments)
                .Include(e => e.Prices)
                .Include(e => e.Contacts)
                .FirstOrDefault(expression);
        }

        public async Task<Distributor> GetEntityAsync(Expression<Func<Distributor, bool>> expression) {
            return await this._context.Distributors
                .Include(e => e.Attachments)
                .Include(e => e.Prices)
                .Include(e => e.Contacts)
                .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Distributor> GetEntityList(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) {
            IQueryable<Distributor> query = this._context.Set<Distributor>()
                .Include(e => e.Attachments)
                .Include(e => e.Prices)
                .Include(e => e.Contacts)
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

        public async Task<IEnumerable<Distributor>> GetEntityListAsync(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) {
            IQueryable<Distributor> query = this._context.Set<Distributor>()
                .Include(e => e.Attachments)
                .Include(e => e.Prices)
                .Include(e => e.Contacts)
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
            this._context.Distributors
                .Include(e => e.Attachments)
                .Include(e => e.Prices)
                .Include(e => e.Contacts)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Distributors
                .Include(e => e.Attachments)
                .Include(e => e.Prices)
                .Include(e => e.Contacts)
                .LoadAsync();
        }
    }
}
