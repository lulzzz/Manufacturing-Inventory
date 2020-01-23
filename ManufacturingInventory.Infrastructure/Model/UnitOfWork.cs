using ManufacturingInventory.Infrastructure.Model.DbContextExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model {
    public class UnitOfWork : IUnitOfWork,IDisposable {

        private ManufacturingContext _context;
        private bool _disposed = false;

        public UnitOfWork(ManufacturingContext context) {
            this._context = context;
        }

        public async Task<int> Save() {
            int affectedRows = await _context.SaveChangesAsync();
            return affectedRows;
        }

        public async Task Undo() {
            await Task.Run(() => {
                this._context.UndoDbContext();
            });
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing) {
            if (!this._disposed) {
                if (disposing) {
                    this._context.Dispose();
                }
            }
            this._disposed = true;
        }
    }

    public class UnitOfWorkV2 : IUnitOfWorkV2, IDisposable {

        private ManufacturingContext _context;
        private bool _disposed = false;

        public UnitOfWorkV2(ManufacturingContext context) {
            this._context = context;
        }


        public int Save() {
            return this._context.SaveChanges();
        }

        public async Task<int> SaveAsync() {
            int affectedRows = await _context.SaveChangesAsync();
            return affectedRows;
        }

        public void Undo() {
            this._context.UndoDbContext();
        }

        public async Task UndoAsync() {
            await Task.Run(() => {
                this._context.UndoDbContext();
            });
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing) {
            if (!this._disposed) {
                if (disposing) {
                    this._context.Dispose();
                }
            }
            this._disposed = true;
        }
    }
}
