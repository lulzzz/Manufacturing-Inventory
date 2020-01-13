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
    public class TransactionService : IEntityService<Transaction> {

        private ManufacturingContext _context;

        public TransactionService(ManufacturingContext context) {
            this._context = context;
        }

        public Transaction Add(Transaction entity, bool save) {
            var transaction =this.GetEntity(e => e.Id == entity.Id, true);
            if (transaction != null) {
                return null;
            }
            var added = this._context.Transactions.Add(entity).Entity;
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

        public async Task<Transaction> AddAsync(Transaction entity, bool save) {
            var transaction = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (transaction != null) {
                return null;
            }
            var added = (await this._context.Transactions.AddAsync(entity)).Entity;
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
        
        public Transaction Update(Transaction entity, bool save) {
            var transaction = this.GetEntity(e => e.Id == entity.Id, true);
            if (transaction == null) {
                return null;
            }
            transaction.Set(entity);
            var updated =this._context.Transactions.Update(transaction).Entity;
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
        
        
        public async Task<Transaction> UpdateAsync(Transaction entity, bool save) {
            var transaction = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (transaction == null) {
                return null;
            }
            transaction.Set(entity);
            var updated = (await Task.Run(() => this._context.Transactions.Update(transaction))).Entity;
            if (save) {
                if (await this.SaveChangesAsync()) {
                    return updated;
                } else {
                    return null;
                }
            } else {
                return updated;
            }
        }
       
        
        public Transaction Delete(Transaction entity, bool save) {
            var transaction = this.GetEntity(e => e.Id == entity.Id, true);
            if (transaction == null) {
                return null;
            }
            var removed = this._context.Transactions.Remove(transaction).Entity;
            if (save) {
                if (this.SaveChanges()) {
                    return removed;
                } else {
                    return null;
                }
            } else {
                return removed;
            }
        }


        public async Task<Transaction> DeleteAsync(Transaction entity, bool save) {
            var transaction = await this.GetEntityAsync(e => e.Id == entity.Id, true);
            if (transaction == null) {
                return null;
            }
            var removed = (await Task.Run(() => this._context.Transactions.Remove(transaction))).Entity;
            if (save) {
                if(await this.SaveChangesAsync()) {
                    return removed;
                } else {
                    return null;
                }
            } else {
                return removed;
            }
        }

        public Transaction GetEntity(Expression<Func<Transaction, bool>> expression, bool tracking) {
            if (tracking) {
                return this._context.Transactions
                    .AsNoTracking()
                    .Include(e => e.Session)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.Part)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.BubblerParameter)
                    .Include(e => e.Location)
                    .Include(e => e.ReferenceTransaction)
                    .FirstOrDefault(expression);
            } else {
                return this._context.Transactions
                    .Include(e => e.Session)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.Part)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.BubblerParameter)
                    .Include(e => e.Location)
                    .Include(e => e.ReferenceTransaction)
                    .FirstOrDefault(expression);
            }
        }


        public async Task<Transaction> GetEntityAsync(Expression<Func<Transaction, bool>> expression, bool tracking) {
            if (tracking) {
                return await this._context.Transactions
                    .AsNoTracking()
                    .Include(e => e.Session)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.Part)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.BubblerParameter)
                    .Include(e => e.Location)
                    .Include(e => e.ReferenceTransaction)
                    .FirstOrDefaultAsync(expression);
            } else {
                return await this._context.Transactions
                    .Include(e => e.Session)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.Part)
                    .Include(e => e.PartInstance)
                        .ThenInclude(e => e.BubblerParameter)
                    .Include(e => e.Location)
                    .Include(e => e.ReferenceTransaction)
                    .FirstOrDefaultAsync(expression);
            }
        }
        
        public IEnumerable<Transaction> GetEntityList(Expression<Func<Transaction, bool>> expression = null, Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy = null) {
            IQueryable<Transaction> query = this._context.Set<Transaction>()
                .AsNoTracking()
                .Include(e => e.Session)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.Part)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.BubblerParameter)
                .Include(e => e.Location)
                .Include(e => e.ReferenceTransaction);

            if (expression != null) {
                query = query.Where(expression);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<Transaction>> GetEntityListAsync(Expression<Func<Transaction, bool>> expression = null, Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy = null) {
            IQueryable<Transaction> query = this._context.Set<Transaction>()
                .AsNoTracking()
                .Include(e => e.Session)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.Part)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.BubblerParameter)
                .Include(e => e.Location)
                .Include(e => e.ReferenceTransaction);

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
            this._context.Transactions
                .Include(e => e.Session)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.Part)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.BubblerParameter)
                .Include(e => e.Location)
                .Include(e => e.ReferenceTransaction)
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
