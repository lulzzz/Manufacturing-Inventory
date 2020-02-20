using ManufacturingInventory.Application.Boundaries.PartInstanceTableView;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class PartInstanceTableViewUseCase : IPartInstanceTableViewUseCase {
        private ManufacturingContext _context;
        private IEntityProvider<PartInstance> _partInstanceProvider;

        public PartInstanceTableViewUseCase(ManufacturingContext context) {
            this._context = context;
            this._partInstanceProvider = new PartInstanceProvider(context);
        }

        public async Task<PartInstance> GetPartInstance(int partInstanceId) {
            return await this._partInstanceProvider.GetEntityAsync(e => e.Id == partInstanceId);
        }

        public async Task<IEnumerable<PartInstance>> GetPartInstances(int partId) {
            return await this._partInstanceProvider.GetEntityListAsync(e => e.PartId == partId);
        }

        public async Task Load() {
            await this._partInstanceProvider.LoadAsync();
        }
    }
}
