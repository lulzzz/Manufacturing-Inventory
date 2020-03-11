using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;

namespace ManufacturingInventory.Application.UseCases {
    public class DistributorNavigation : IDistributornNavigationUseCase {
        private ManufacturingContext _context;
        private IEntityProvider<Distributor> _distributorProvider;

        public DistributorNavigation(ManufacturingContext context) {
            this._context = context;
            this._distributorProvider = new DistributorProvider(context);
        }

        public async Task<IEnumerable<Distributor>> GetDistributors() {
            return await this._distributorProvider.GetEntityListAsync();
        }
        public async Task Load() {
            await this._distributorProvider.LoadAsync();
        }
    }
}
