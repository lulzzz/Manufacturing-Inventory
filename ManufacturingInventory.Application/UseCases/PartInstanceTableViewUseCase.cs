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

        public async Task<bool> CanReturnInstance(int partInstanceId) {
            var partInstance=await this._partInstanceProvider.GetEntityAsync(e => e.Id == partInstanceId);
            if (partInstance != null) {
                var newest = (await this._transactionProvider.GetEntityListAsync(e => e.PartInstanceId == partInstance.Id  && e.InventoryAction==InventoryAction.OUTGOING)).OrderByDescending(e=>e.TimeStamp).FirstOrDefault();
                if (newest != null) {
                    //if()
                } else {
                    return false;
                }
            } else {
                return false;
            }

        }

        public async Task Load() {
            await this._partInstanceProvider.LoadAsync();
        }
    }
}
