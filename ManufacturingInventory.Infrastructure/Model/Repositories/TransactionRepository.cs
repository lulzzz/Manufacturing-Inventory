using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model.Repositories {
    public class TransactionRepository : IRepository<Transaction> {

        private ManufacturingContext _context;

        public TransactionRepository(ManufacturingContext context) {
            this._context = context;
        }

        public Transaction Add(Transaction entity) {
            var transaction = this.GetEntity(e => e.Id == entity.Id);
            if (transaction != null) {
                return null;
            }
            return this._context.Transactions.Add(entity).Entity;
        }

        public async Task<Transaction> AddAsync(Transaction entity) {
            var transaction = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (transaction != null) {
                return null;
            }
            return (await this._context.Transactions.AddAsync(entity)).Entity;
        }

        public Transaction Update(Transaction entity) {
            var transaction = this.GetEntity(e => e.Id == entity.Id);
            if (transaction == null) {
                return null;
            }
            transaction.Set(entity);
            return this._context.Transactions.Update(transaction).Entity;
        }

        public async Task<Transaction> UpdateAsync(Transaction entity) {
            var transaction = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (transaction == null) {
                return null;
            }
            transaction.Set(entity);
            return (await Task.Run(() => this._context.Transactions.Update(transaction))).Entity;
        }


        public Transaction Delete(Transaction entity) {
            var transaction = this.GetEntity(e => e.Id == entity.Id);
            if (transaction == null) {
                return null;
            }
           return this._context.Transactions.Remove(transaction).Entity;
        }

        public async Task<Transaction> DeleteAsync(Transaction entity) {
            var transaction = await this.GetEntityAsync(e => e.Id == entity.Id);
            if (transaction == null) {
                return null;
            }
            return (await Task.Run(() => this._context.Transactions.Remove(transaction))).Entity;
        }
        public Transaction GetEntity(Expression<Func<Transaction, bool>> expression) {
                return this._context.Transactions
                    .Include(e => e.Session)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.Part)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.BubblerParameter)
                    .Include(e => e.Location)
                    .Include(e => e.ReferenceTransaction)
                    .AsNoTracking()
                    .FirstOrDefault(expression);
        }


        public async Task<Transaction> GetEntityAsync(Expression<Func<Transaction, bool>> expression) {

                return await this._context.Transactions
                    .Include(e => e.Session)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.Part)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.BubblerParameter)
                    .Include(e => e.Location)
                    .Include(e => e.ReferenceTransaction)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(expression);
        }

        public IEnumerable<Transaction> GetEntityList(Expression<Func<Transaction, bool>> expression = null, Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy = null) {
            IQueryable<Transaction> query = this._context.Set<Transaction>()
                .Include(e => e.Session)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.Part)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.BubblerParameter)
                .Include(e => e.Location)
                .Include(e => e.ReferenceTransaction)
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

        public async Task<IEnumerable<Transaction>> GetEntityListAsync(Expression<Func<Transaction, bool>> expression = null, Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy = null) {
            IQueryable<Transaction> query = this._context.Set<Transaction>()
                .Include(e => e.Session)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.Part)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.BubblerParameter)
                .Include(e => e.Location)
                .Include(e => e.ReferenceTransaction)
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
            this._context.Transactions
                .AsNoTracking()
                .Include(e => e.Session)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.Part)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.BubblerParameter)
                .Include(e => e.Location)
                .Include(e => e.ReferenceTransaction)
                .AsNoTracking()
                .Load();
        }

        public async Task LoadAsync() {
            await this._context.Transactions
                .Include(e => e.Session)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.Part)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.BubblerParameter)
                .Include(e => e.Location)
                .Include(e => e.ReferenceTransaction)
                .AsNoTracking()
                .LoadAsync();
        }

    }
}
