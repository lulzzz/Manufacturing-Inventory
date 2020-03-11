using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Providers {
    public class PriceProvider : IEntityProvider<Price> {
        private ManufacturingContext _context;

        public PriceProvider(ManufacturingContext context) {
            this._context = context;
        }

        public Price GetEntity(Expression<Func<Price, bool>> expression) {
            return this._context.Prices
                .Include(e => e.Distributor)
                .Include(e => e.Attachment)
                .Include(e => e.PartInstances)
                .Include(e => e.PartPrices)
                .Include(e => e.PriceLogs)
                .FirstOrDefault(expression);
        }

        public async Task<Price> GetEntityAsync(Expression<Func<Price, bool>> expression) {
            return await this._context.Prices
                .Include(e => e.Distributor)
                .Include(e => e.Attachment)
                .Include(e => e.PartInstances)
                .Include(e => e.PartPrices)
                .Include(e => e.PriceLogs)
                .FirstOrDefaultAsync(expression);
        }


        public IEnumerable<Price> GetEntityList(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) {
            IQueryable<Price> query = this._context.Set<Price>()
                .Include(e => e.Distributor)
                .Include(e => e.Attachment)
                .Include(e => e.PartInstances)
                .Include(e => e.PartPrices)
                .Include(e => e.PriceLogs)
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

        public async Task<IEnumerable<Price>> GetEntityListAsync(Expression<Func<Price, bool>> expression = null, Func<IQueryable<Price>, IOrderedQueryable<Price>> orderBy = null) {
            IQueryable<Price> query = this._context.Set<Price>()
                .Include(e => e.Distributor)
                .Include(e => e.Attachment)
                .Include(e => e.PartInstances)
                .Include(e => e.PartPrices)
                .Include(e => e.PriceLogs)
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
            this._context.Prices
                .Include(e => e.Distributor)
                .Include(e => e.Attachment)
                .Include(e => e.PartInstances)
                .Include(e => e.PartPrices)
                .Include(e => e.PriceLogs)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Prices
                .Include(e => e.Distributor)
                .Include(e => e.Attachment)
                .Include(e => e.PartInstances)
                .Include(e => e.PartPrices)
                .Include(e => e.PriceLogs)
                .LoadAsync();
        }
    }
}
