using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class DistributorEdit : IDistributorEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Distributor> _distributorRepository;
        private IEntityProvider<Price> _priceProvider;
        private IEntityProvider<Contact> _contactProvider;

        public DistributorEdit(ManufacturingContext context) {
            this._context = context;
            this._distributorRepository = new DistributorRepository(context);
            this._priceProvider = new PriceProvider(context);
            this._contactProvider = new ContactProvider(context);
        }



        public Task<DistributorEditOutput> Execute(DistributorEditInput input) => throw new NotImplementedException();


        public async Task<IEnumerable<Contact>> GetContacts(int distributorId) {
            return await this._contactProvider.GetEntityListAsync(e=>e.DistributorId==distributorId);
        }

        public async Task<Distributor> GetDistributor(int distributorId) {
            return await this._distributorRepository.GetEntityAsync(e => e.Id == distributorId);
        }

        public async Task<IEnumerable<Price>> GetPrices(int distributorId) {
            return await this._priceProvider.GetEntityListAsync(e => e.DistributorId == distributorId);
        }

        public async Task Load() {
            await this._distributorRepository.LoadAsync();
            await this._priceProvider.LoadAsync();
            await this._contactProvider.LoadAsync();
        }
    }
}
