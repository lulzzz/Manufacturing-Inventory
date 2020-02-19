using ManufacturingInventory.Application.Boundaries.CheckIn;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
            switch (input.PricingOption) {
                case PriceOption.CreateNew:
                    return await this.ExecuteNewPrice(input);
                case PriceOption.UseExisting:
                    return await this.ExecuteExistingPrice(input);
                case PriceOption.NoPrice:
                    return await this.ExecuteNoPrice(input);
                default:
                    return new CheckInOutput(null, false, "Internal Error: Not a Valid Price Option");
            }
        }

        public async Task<CheckInOutput> ExecuteNewPrice(CheckInInput input) {
            var part = await this._partRepository.GetEntityAsync(e => e.Id == input.PartId);
            if (part != null) {
                input.PartInstance.PartId = input.PartId;
                input.PartInstance.Price = input.Price;
                input.PartInstance.UpdatePrice();
                var instanceEntity = await this._partInstanceRepository.AddAsync(input.PartInstance);
                if (instanceEntity != null) {
                    PartPrice partPrice = new PartPrice(part, instanceEntity.Price);
                    PriceLog priceLog = new PriceLog(instanceEntity, instanceEntity.Price);
                    await this._partPriceRepository.AddAsync(partPrice);
                    await this._priceLogRepository.AddAsync(priceLog);
                    Transaction transaction = new Transaction();
                    transaction.SetupCheckinBubbler(instanceEntity, InventoryAction.INCOMING, instanceEntity.LocationId, input.TimeStamp);
                    transaction.SessionId = this._userService.CurrentSession.Id;
                    var tranEntity = await this._transactionRepository.AddAsync(transaction);
                    var count =await this._unitOfWork.Save();
                    if (count > 0) {
                        return new CheckInOutput(instanceEntity, true, "Part Checked In!");
                    } else {
                        await this._unitOfWork.Undo();
                        return new CheckInOutput(null, false, "Error: Check in Failed");
                    }

                } else {
                    await this._unitOfWork.Undo();
                    return new CheckInOutput(null, false, "Error: Could Not Create Part Instance");
                }

            } else {
                await this._unitOfWork.Undo();
                return new CheckInOutput(null, false, "Error: Part Not Found");
            }
        }

        public async Task<CheckInOutput> ExecuteNoPrice(CheckInInput input) {
            var part = await this._partRepository.GetEntityAsync(e => e.Id == input.PartId);
            if (part != null) {
                input.PartInstance.PartId = input.PartId;
                var instanceEntity = await this._partInstanceRepository.AddAsync(input.PartInstance);
                if (instanceEntity != null) {
                    Transaction transaction = new Transaction();
                    transaction.SetupCheckinBubbler(instanceEntity, InventoryAction.INCOMING, instanceEntity.LocationId, input.TimeStamp);
                    transaction.SessionId = this._userService.CurrentSession.Id;
                    var tranEntity = await this._transactionRepository.AddAsync(transaction);
                    var count = await this._unitOfWork.Save();
                    if (count > 0) {
                        return new CheckInOutput(instanceEntity, true, "Part Checked In!");
                    } else {
                        await this._unitOfWork.Undo();
                        return new CheckInOutput(null, false, "Error: Check in Failed");
                    }

                } else {
                    await this._unitOfWork.Undo();
                    return new CheckInOutput(null, false, "Error: Could Not Create Part Instance");
                }
            } else {
                await this._unitOfWork.Undo();
                return new CheckInOutput(null, false, "Error: Part Not Found");
            }
        }

        public async Task<CheckInOutput> ExecuteExistingPrice(CheckInInput input) {
            var part = await this._partRepository.GetEntityAsync(e => e.Id == input.PartId);
            if (part != null) {
                input.PartInstance.PartId = input.PartId;
                input.PartInstance.UpdatePrice(input.Price.Id,input.Price.UnitCost);
                var instanceEntity = await this._partInstanceRepository.AddAsync(input.PartInstance);
                if (instanceEntity != null) {
                    PriceLog priceLog = new PriceLog();
                    priceLog.TimeStamp = input.TimeStamp;
                    priceLog.IsCurrent = true;
                    priceLog.PartInstance = instanceEntity;
                    //priceLog.Price = input.Price;
                    priceLog.PriceId = input.Price.Id;
                    await this._priceLogRepository.AddAsync(priceLog);
                    Transaction transaction = new Transaction();
                    transaction.SetupCheckinBubbler(instanceEntity, InventoryAction.INCOMING, instanceEntity.LocationId, input.TimeStamp);
                    transaction.SessionId = this._userService.CurrentSession.Id;
                    var tranEntity = await this._transactionRepository.AddAsync(transaction);
                    var count = await this._unitOfWork.Save();
                    if (count > 0) {
                        return new CheckInOutput(instanceEntity, true, "Part Checked In!");
                    } else {
                        await this._unitOfWork.Undo();
                        return new CheckInOutput(null, false, "Error: Check in Failed");
                    }

                } else {
                    await this._unitOfWork.Undo();
                    return new CheckInOutput(null, false, "Error: Could Not Create Part Instance");
                }

            } else {
                await this._unitOfWork.Undo();
                return new CheckInOutput(null, false, "Error: Part Not Found");
            }
        }

        //public async Task<CheckInOutput> ExecuteBubbler(CheckInInput input) {
        //    if (input.Price != null && !input.CreateNewPrice) {
        //        input.PartInstance.PriceId = input.Price.Id;
        //    }
        //    var entity = await this._partInstanceRepository.AddAsync(input.PartInstance);
        //    if (entity != null) {
        //        if (input.CreateNewPrice) {
        //            var part = await this._partRepository.GetEntityAsync(e => e.Id == entity.PartId);
        //            if (part != null) {
        //                if (input.Price != null) {
        //                    PartPrice partPrice = new PartPrice(part, entity.Price);
        //                    PriceLog priceLog = new PriceLog(entity, entity.Price);
        //                    entity.UnitCost = input.Price.UnitCost;
        //                    entity.TotalCost = (entity.BubblerParameter.NetWeight * entity.UnitCost) * entity.Quantity;
        //                    await this._partPriceRepository.AddAsync(partPrice);
        //                    await this._priceLogRepository.AddAsync(priceLog);
        //                } else {
        //                    await this._unitOfWork.Undo();
        //                    return new CheckInOutput(null, false, "Error:  No Price given when marked as CreateNewPrice");
        //                }
        //                Transaction transaction = new Transaction();
        //                transaction.SetupCheckinBubbler(entity, InventoryAction.INCOMING, entity.CurrentLocation, input.TimeStamp);
        //                transaction.SessionId = this._userService.CurrentSession.Id;
        //                var tranEntity = await this._transactionRepository.AddAsync(transaction);
        //                if (tranEntity != null) {
        //                    await this._unitOfWork.Save();
        //                    return new CheckInOutput(entity, true, "Success: Check in completed");
        //                } else {
        //                    await this._unitOfWork.Undo();
        //                    return new CheckInOutput(null, false, "Error: Check in Failed");
        //                }
        //            } else {
        //                await this._unitOfWork.Undo();
        //                return new CheckInOutput(null, false, "Error: Part Not Found,Cannot Create Pricing");
        //            }
        //        } else {
        //            if (input.Price!=null) {
        //                var priceEntity = await this._priceRepository.GetEntityAsync(e => e.Id == input.Price.Id);
        //                if (priceEntity != null) {
        //                    entity.UnitCost = priceEntity.UnitCost;
        //                    entity.TotalCost = (entity.BubblerParameter.NetWeight * entity.UnitCost) * entity.Quantity;
        //                    PriceLog priceLog = new PriceLog();
        //                    priceLog.TimeStamp = input.TimeStamp;
        //                    priceLog.IsCurrent = true;
        //                    priceLog.PartInstance = entity;
        //                    priceLog.Price = priceEntity;
        //                    await this._priceLogRepository.AddAsync(priceLog);
        //                } else {
        //                    await this._unitOfWork.Undo();
        //                    return new CheckInOutput(null, false, "Error: Existing Price Not Found");
        //                }
        //            }
        //            Transaction transaction = new Transaction();
        //            transaction.SetupCheckinBubbler(entity, InventoryAction.INCOMING, entity.LocationId, input.TimeStamp);
        //            transaction.SessionId = this._userService.CurrentSession.Id;
        //            var tranEntity = await this._transactionRepository.AddAsync(transaction);
        //            if (tranEntity != null) {                                
        //                await this._unitOfWork.Save();
        //                return new CheckInOutput(entity, true, "Success: Check in completed");
        //            } else {
        //                await this._unitOfWork.Undo();
        //                return new CheckInOutput(null, false, "Error: Check in Failed");
        //            }
        //        }
        //    } else {
        //        return new CheckInOutput(null, false, "Error Creating PartInstance");
        //    }
        //}

        public async Task<CheckInOutput> ExecuteStandard(CheckInInput input) {
            return await Task.Run(() => new CheckInOutput(null, false, "Error: Not Implemented Yet"));
        }

        public async Task<IEnumerable<Price>> GetAvailablePrices(int partId) {
            return (await this._partPriceRepository.GetEntityListAsync(e => e.PartId == partId)).Select(e => e.Price);
        }

        public async Task<IEnumerable<Category>> GetCategories() {
            return await this._categoryProvider.GetEntityListAsync();
        }

        public async Task<IEnumerable<Distributor>> GetDistributors() {
            return await this._distributorProvider.GetEntityListAsync();
        }

        public async Task<Price> GetPrice(int priceId) {
            return await this._priceRepository.GetEntityAsync(e => e.Id == priceId);
        }

        public async Task<IEnumerable<Warehouse>> GetWarehouses() {
            return (await this._locationProvider.GetEntityListAsync()).OfType<Warehouse>();
        }
    }
}
