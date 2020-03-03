using ManufacturingInventory.Application.Boundaries.PartInstanceTableView;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class PartInstanceTableViewUseCase : IPartInstanceTableViewUseCase {
        private ManufacturingContext _context;
        private IEntityProvider<PartInstance> _partInstanceProvider;
        private IEntityProvider<Transaction> _transactionProvider;

        public PartInstanceTableViewUseCase(ManufacturingContext context) {
            this._context = context;
            this._partInstanceProvider = new PartInstanceProvider(context);
            this._transactionProvider = new TransactionProvider(context);
        }

        public async Task<PartInstance> GetPartInstance(int partInstanceId) {
            return await this._partInstanceProvider.GetEntityAsync(e => e.Id == partInstanceId);
        }

        public async Task<IEnumerable<PartInstance>> GetPartInstances(int partId) {
            return await this._partInstanceProvider.GetEntityListAsync(e => e.PartId == partId);
        }

        public async Task<Transaction> GetLastOutgoing(int partInstanceId) {
            //var partInstance = await this._partInstanceProvider.GetEntityAsync(e => e.Id == partInstanceId);
            var transactions = await this._transactionProvider.GetEntityListAsync(e => e.PartInstanceId == partInstanceId);
            var lastOutgoing = transactions.OrderByDescending(e => e.TimeStamp).FirstOrDefault(e => e.InventoryAction == InventoryAction.OUTGOING);
            if (lastOutgoing != null) {
                var returned=transactions.FirstOrDefault(e=>e.ReferenceTransactionId==lastOutgoing.Id);
                if (returned == null) {
                    return lastOutgoing;
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        public async Task Load() {
            await this._partInstanceProvider.LoadAsync();
        }
    }
}
