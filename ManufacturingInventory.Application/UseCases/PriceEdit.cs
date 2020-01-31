using ManufacturingInventory.Application.Boundaries.PriceEdit;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class PriceEdit : IPriceEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Price> _priceRepository;
        private IEntityProvider<Distributor> _distributorProvider;
        private IUnitOfWork _unitOfWork;

        public PriceEdit(ManufacturingContext context) {
            this._context = context;
            this._priceRepository = new PriceRepository(context);
            this._distributorProvider = new DistributorProvider(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public Task<PriceEditOutput> Execute(PriceEditInput input) {
            return null;
        }
       
        public async Task<IEnumerable<Distributor>> GetDistributors() {
            return await this._distributorProvider.GetEntityListAsync();
        }

        public async Task<Price> GetPrice(int priceId) {
            return await this._priceRepository.GetEntityAsync(e => e.Id == priceId);
        }

        public async Task Load() {
            await this._priceRepository.LoadAsync();
        }
    }
}
