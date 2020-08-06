using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class MonthlySummaryRepository : IRepository<MonthlySummary> {
        private ManufacturingContext _context;

        public MonthlySummaryRepository(ManufacturingContext context) {
            this._context = context;
        }

        public MonthlySummary Add(MonthlySummary entity) {
            var monthlySummary = this._context.Attachments.FirstOrDefault(e => e.Id == entity.Id);
            if (monthlySummary != null) {
                return null;
            }
            return this._context.Add(entity).Entity;
        }

        public async Task<MonthlySummary> AddAsync(MonthlySummary entity) {
            var monthlySummary = await this._context.Attachments.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (monthlySummary != null) {
                return null;
            }
            return (await this._context.AddAsync(entity)).Entity;
        }

        public MonthlySummary Update(MonthlySummary entity) {
            var monthlySummary = this._context.MonthlySummaries.FirstOrDefault(e => e.Id == entity.Id);
            if (monthlySummary == null) {
                return null;
            }
            monthlySummary.Set(entity);
            return this._context.Update(monthlySummary).Entity;
        }

        public async Task<MonthlySummary> UpdateAsync(MonthlySummary entity) {
            var monthlySummary = await this._context.MonthlySummaries.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (monthlySummary == null) {
                return null;
            }
            monthlySummary.Set(entity);
            return this._context.Update(monthlySummary).Entity;
        }

        public MonthlySummary Delete(MonthlySummary entity) {
            return this._context.MonthlySummaries.Remove(entity).Entity;
        }

        public async Task<MonthlySummary> DeleteAsync(MonthlySummary entity) {
            var monthlySummary = await Task.Run(() => this._context.MonthlySummaries.Remove(entity).Entity);
            return monthlySummary;
        }

        public MonthlySummary GetEntity(Expression<Func<MonthlySummary, bool>> expression) {
            return this._context.MonthlySummaries.FirstOrDefault(expression);
        }

        public async Task<MonthlySummary> GetEntityAsync(Expression<Func<MonthlySummary, bool>> expression) {
            return await this._context.MonthlySummaries.FirstOrDefaultAsync(expression);
        }

        public IEnumerable<MonthlySummary> GetEntityList(Expression<Func<MonthlySummary, bool>> expression = null,
            Func<IQueryable<MonthlySummary>, IOrderedQueryable<MonthlySummary>> orderBy = null) {

            IQueryable<MonthlySummary> query = this._context.Set<MonthlySummary>()
                .Include(e=>e.MonthlyPartSnapshots)
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

        public async Task<IEnumerable<MonthlySummary>> GetEntityListAsync(Expression<Func<MonthlySummary, bool>> expression = null,
            Func<IQueryable<MonthlySummary>, IOrderedQueryable<MonthlySummary>> orderBy = null) {
            IQueryable<MonthlySummary> query = this._context.Set<MonthlySummary>()
                .Include(e => e.MonthlyPartSnapshots)
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
            this._context.MonthlySummaries
                .Include(e => e.MonthlyPartSnapshots)
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.MonthlySummaries
                .Include(e => e.MonthlyPartSnapshots)
                .LoadAsync();
        }

    }
}
