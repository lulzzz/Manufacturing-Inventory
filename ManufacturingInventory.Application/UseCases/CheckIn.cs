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
            this._transactionRepository = new TransactionRepository(context);
            this._userService = userService;
            this._unitOfWork = new UnitOfWork(context);
            this._context = context;
        }

        public async Task<CheckInOutput> Execute(CheckInInput input) {
            if (input.PartInstance.IsBubbler) {
                return await this.ExecuteBubbler(input);
            } else {
                return await this.ExecuteStandard(input);
            }
        }

        public async Task<CheckInOutput> ExecuteBubbler(CheckInInput input) {
            var entity = await this._partInstanceRepository.AddAsync(input.PartInstance);
            if (entity != null) {
                if (entity.Price != null) {
                    var part = await this._partRepository.GetEntityAsync(e => e.Id == entity.PartId);
                    if (part != null) {
                        PartPrice partPrice = new PartPrice(part, entity.Price);
                        PriceLog priceLog = new PriceLog(entity, entity.Price);
                        await this._partPriceRepository.AddAsync(partPrice);
                        await this._priceLogRepository.AddAsync(priceLog);
                        if (input.CreateTransaction) {
                            Transaction transaction = new Transaction(entity, InventoryAction.INCOMING,
                                entity.BubblerParameter.Measured, entity.BubblerParameter.Weight, 
                                entity.CurrentLocation, input.TimeStamp);
                            transaction.SessionId = this._userService.CurrentSession.Id;
                            var tranEntity = await this._transactionRepository.AddAsync(transaction);
                            if (tranEntity != null) {
                                await this._unitOfWork.Save();
                                return new CheckInOutput(entity, true, "Success: Check in completed"); 
                            } else {
                                await this._unitOfWork.Undo();
                                return new CheckInOutput(null, false, "Error: Check in Failed");
                            }
                        } else {

                        }
                    } else {
                        await this._unitOfWork.Undo();
                        return new CheckInOutput(null, false, "Error: Part Not Found,Cannot Create Pricing");
                    }
                } else {

                }

            } else {
                return new CheckInOutput(null, false, "Error Creating PartInstance");
            }
            return null;
        }

        public async Task<CheckInOutput> ExecuteStandard(CheckInInput input) {
            return null;
        }



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
