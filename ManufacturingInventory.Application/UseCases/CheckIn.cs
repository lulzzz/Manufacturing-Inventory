using ManufacturingInventory.Application.Boundaries.CheckIn;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class CheckIn : ICheckInUseCase {
        private ManufacturingContext _context;
        private IRepository<Transaction> _transactionRepository;

        private IRepository<Part> _partRepository;
        private IRepository<PartInstance> _partInstanceRepository;
        private IRepository<BubblerParameter> _bubblerRepository;

        private IRepository<Price> _priceRepository;
        private IRepository<PartPrice> _partPriceRepository;
        private IRepository<PriceLog> _priceLogRepository;

        private IEntityProvider<Location> _locationProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IEntityProvider<Distributor> _distributorProvider;
        private IEntityProvider<PartType> _partTypeProvider;

        private IUserService _userService;
        private IUnitOfWork _unitOfWork;

        public CheckIn(ManufacturingContext context,IUserService userService) {
            this._priceRepository = new PriceRepository(context);
            this._partPriceRepository = new PartPriceRepository(context);
            this._priceLogRepository = new PriceLogRepository(context);
            this._partInstanceRepository = new PartInstanceRepository(context);
            this._bubblerRepository = new BubblerParameterRepository(context);
            this._partRepository = new PartRepository(context);
            this._categoryProvider = new CategoryProvider(context);
            this._locationProvider = new LocationProvider(context);
            this._distributorProvider = new DistributorProvider(context);
            this._partTypeProvider = new PartTypeProvider(context);
            this._transactionRepository = new TransactionRepository(context);
            this._userService = userService;
            this._unitOfWork = new UnitOfWork(context);
            this._context = context;
        }

        public Task<CheckInOutput> Execute(CheckInInput input) => throw new NotImplementedException();



        public async Task<IEnumerable<Category>> GetCategories() {
            return await this._categoryProvider.GetEntityListAsync();
        }

        public async Task<IEnumerable<Distributor>> GetDistributors() {
            return await this._distributorProvider.GetEntityListAsync();
        }

        public async Task<IEnumerable<Location>> GetLocations() {
            return await this._locationProvider.GetEntityListAsync();
        }
    }
}
