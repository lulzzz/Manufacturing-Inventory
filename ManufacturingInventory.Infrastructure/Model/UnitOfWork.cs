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

        }

        public void Dispose() {
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
