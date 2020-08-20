using ManufacturingInventory.Application.Boundaries.CheckIn;
using ManufacturingInventory.Domain.Security.Interfaces;
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
        private IRepository<Category> _categoryRepository;
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
            this._categoryRepository = new CategoryRepository(context);
            this._locationProvider = new LocationProvider(context);
            this._distributorProvider = new DistributorProvider(context);
            this._transactionRepository = new TransactionRepository(context);
            this._unitOfWork = new UnitOfWork(context);
            this._userService = userService;
            this._context = context;
        }

        public async Task<CheckInOutput> Execute(CheckInInput input) {
            if (input.IsExisiting) {
                return await this.ExecuteAddToExisiting(input);
            } else {
                switch (input.PricingOption) {
                    case PriceOption.CreateNew:
                        return await this.ExecuteNewPrice(input);
                    case PriceOption.UseExisting:
                        return await this.ExecuteExistingPrice(input);
                    case PriceOption.NoPrice:
                        return await this.ExecuteNoPrice(input);
                    default:
                        return new CheckInOutput(null, false, "Internal Error: Not a Valid Price Option" + Environment.NewLine + "Please Contact Administrator");
                }
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
                    transaction.SetupCheckIn(instanceEntity, InventoryAction.INCOMING, instanceEntity.LocationId, input.TimeStamp);
                    transaction.SessionId = this._userService.CurrentSessionId.Value;
                    var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == instanceEntity.StockTypeId);
                    if (!stockType.IsDefault) {
                        if (stockType != null) {
                            if (instanceEntity.IsBubbler) {
                                stockType.Quantity += (int)instanceEntity.BubblerParameter.Weight;
                            } else {
                                stockType.Quantity += instanceEntity.Quantity;
                            }
                            await this._categoryRepository.UpdateAsync(stockType);
                        }
                    } else {
                        IndividualAlert alert = new IndividualAlert();
                        alert.PartInstance = instanceEntity;
                        instanceEntity.IndividualAlert=alert;
                        this._context.Add(alert);
                    }

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
                    transaction.SetupCheckIn(instanceEntity, InventoryAction.INCOMING, instanceEntity.LocationId, input.TimeStamp);
                    transaction.SessionId = this._userService.CurrentSessionId.Value;

                    var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == instanceEntity.StockTypeId);
                    if (!stockType.IsDefault) {
                            if (instanceEntity.IsBubbler) {
                                stockType.Quantity += (int)instanceEntity.BubblerParameter.Weight;
                            } else {
                                stockType.Quantity += instanceEntity.Quantity;
                            }
                            await this._categoryRepository.UpdateAsync(stockType);
                    } else {
                        IndividualAlert alert = new IndividualAlert();
                        alert.PartInstance = instanceEntity;
                        instanceEntity.IndividualAlert = alert;
                        this._context.Add(alert);
                    }

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
                    priceLog.PriceId = input.Price.Id;
                    await this._priceLogRepository.AddAsync(priceLog);
                    Transaction transaction = new Transaction();
                    transaction.SetupCheckIn(instanceEntity, InventoryAction.INCOMING, instanceEntity.LocationId, input.TimeStamp);
                    transaction.SessionId = this._userService.CurrentSessionId.Value;

                    var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == instanceEntity.StockTypeId);
                    if (!stockType.IsDefault) {
                        if (instanceEntity.IsBubbler) {
                            stockType.Quantity += (int)instanceEntity.BubblerParameter.Weight;
                        } else {
                            stockType.Quantity += instanceEntity.Quantity;
                        }
                        await this._categoryRepository.UpdateAsync(stockType);
                    } else {
                        IndividualAlert alert = new IndividualAlert();
                        alert.PartInstance = instanceEntity;
                        instanceEntity.IndividualAlert = alert;
                        this._context.Add(alert);
                    }

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

        public async Task<CheckInOutput> ExecuteStandard(CheckInInput input) {
            return await Task.Run(() => new CheckInOutput(null, false, "Error: Not Implemented Yet"));
        }

        private async Task<CheckInOutput> ExecuteAddToExisiting(CheckInInput input) {
            var partInstance = await this._partInstanceRepository.GetEntityAsync(e => e.Id == input.PartInstance.Id);
            if (partInstance != null) {
                partInstance.UpdateQuantity(input.Quantity.Value);
                Transaction transaction = new Transaction(partInstance, InventoryAction.INCOMING,0, 0, partInstance.CurrentLocation, input.TimeStamp);
                transaction.Quantity = input.Quantity.Value;
                transaction.UnitCost = partInstance.UnitCost;
                transaction.TotalCost = transaction.UnitCost*transaction.Quantity;
                transaction.SessionId = this._userService.CurrentSessionId.Value;

                var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == partInstance.StockType.Id);
                if (!stockType.IsDefault) {
                    if (!partInstance.IsBubbler) {
                        stockType.Quantity += partInstance.Quantity;
                        await this._categoryRepository.UpdateAsync(stockType);
                    }
                } else {
                    IndividualAlert alert = new IndividualAlert();
                    alert.PartInstance = partInstance;
                    partInstance.IndividualAlert = alert;
                    this._context.Add(alert);
                }

                var instance = await this._partInstanceRepository.UpdateAsync(partInstance);
                var trans = await this._transactionRepository.AddAsync(transaction);

                StringBuilder builder = new StringBuilder();
                if (instance != null && trans != null) {
                    var val = await this._unitOfWork.Save();
                    builder.AppendFormat("Success: {0} added to {1}",instance.Name,trans.Quantity);
                    return new CheckInOutput(instance, true, builder.ToString());
                } else {
                    await this._unitOfWork.Undo();
                    builder.AppendFormat("Error: Check In Failed, Please Contact Admin").AppendLine();
                    return new CheckInOutput(instance, false, builder.ToString());
                }
            } else {
                return new CheckInOutput(null, false, "Error: Part Instance not found");
            }
        }

        public async Task<IEnumerable<Price>> GetAvailablePrices(int partId) {
            return (await this._partPriceRepository.GetEntityListAsync(e => e.PartId == partId)).Select(e => e.Price);
        }

        public async Task<IEnumerable<Category>> GetCategories() {
            return await this._categoryRepository.GetEntityListAsync();
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

        public async Task<Part> GetPart(int partId) {
            var part = await this._partRepository.GetEntityAsync(e => e.Id == partId);
            if (part != null) {
                return part;
            } else {
                return null;
            }
        }

        public async Task<PartInstance> GetExisitingPartInstance(int instanceId) {
            return await this._partInstanceRepository.GetEntityAsync(e => e.Id == instanceId);
        }
    }
}
