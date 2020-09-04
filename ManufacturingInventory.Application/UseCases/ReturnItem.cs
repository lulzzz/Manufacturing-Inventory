using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.ReturnItem;
using ManufacturingInventory.Domain.Security.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Providers;

namespace ManufacturingInventory.Application.UseCases {
    public class ReturnItem : IReturnItemUseCase {

        private ManufacturingContext _context;
        private IRepository<Transaction> _transactionRepository;
        private IEntityProvider<Location> _locationProvider;
        private IRepository<Category> _categoryRepository;
        private IRepository<PartInstance> _partInstanceRepository;
        private IRepository<BubblerParameter> _bubblerRepository;
        private IEntityProvider<Part> _partProvider;
        private IUserService _userService;
        private IUnitOfWork _unitOfWork;

        public ReturnItem(ManufacturingContext context, IUserService userService) {
            this._bubblerRepository = new BubblerParameterRepository(context);
            this._transactionRepository = new TransactionRepository(context);
            this._locationProvider = new LocationProvider(context);
            this._categoryRepository = new CategoryRepository(context);
            this._partInstanceRepository = new PartInstanceRepository(context);
            this._partProvider = new PartProvider(context);
            this._userService = userService;
            this._unitOfWork = new UnitOfWork(context);
            this._context = context;
        }

        public async Task<IEnumerable<Condition>> GetConditions() {
            return (await this._categoryRepository.GetEntityListAsync()).OfType<Condition>();
        }
        
        public async Task<PartInstance> GetPartInstance(int instanceId) {
            return await this._partInstanceRepository.GetEntityAsync(e => e.Id == instanceId);
        }
        
        public async Task<int> GetPartWarehouseId(int partId) {
            var part =await this._partProvider.GetEntityAsync(e => e.Id == partId);
            if(part!=null) {
                if (part.WarehouseId.HasValue) {
                    return part.WarehouseId.Value;
                } else {
                    return 0;
                }
            } else {
                return 0;
            }
        }

        public async Task<IEnumerable<Warehouse>> GetWarehouses() {
            return (await this._locationProvider.GetEntityListAsync()).OfType<Warehouse>();
        }

        public async Task<Tuple<double,double>> GetInstanceNetGross(int partInstanceId){
            var instance = (await this._partInstanceRepository.GetEntityAsync(e => e.Id == partInstanceId));
            return new Tuple<double, double>(instance.BubblerParameter.NetWeight,instance.BubblerParameter.GrossWeight);
        }

        public async Task<ReturnItemOutput> Execute(ReturnItemInput input) {

            var partInstance = await this._partInstanceRepository.GetEntityAsync(e => e.Id == input.PartInstanceId);
            var location = await this._locationProvider.GetEntityAsync(e => e.Id == input.LocationId);
            var refTransaction = await this._transactionRepository.GetEntityAsync(e => e.Id == input.ReferenceTransactionId);
            //var bubblerParam
            if (partInstance != null && location != null  && refTransaction!=null) {
                if (input.IsBubbler) {
                    return await this.ExecuteBubbler(partInstance, location,refTransaction, input);
                } else {
                    return await this.ExecuteStandard(partInstance, location, refTransaction, input);
                }
            } else {
                return new ReturnItemOutput(null, false, "PartInstance or Location Not Found");
            }
        }

        private async Task<ReturnItemOutput> ExecuteBubbler(PartInstance partInstance, Location location,Transaction refTransaction, ReturnItemInput item) {
            partInstance.UpdateWeight(item.MeasuredWeight);
            //partInstance.BubblerParameter.DateRemoved = item.TimeStamp;
            partInstance.UpdateQuantity(1);
            partInstance.CostReported = false;
            partInstance.LocationId = location.Id;
            partInstance.DateRemoved = item.TimeStamp;
            if (item.ConditionId != 0) {
                var condition = await this._categoryRepository.GetEntityAsync(e => e.Id == item.ConditionId);
                if (condition != null) {
                    partInstance.ConditionId = condition.Id;
                }
            }

            if (!partInstance.StockType.IsDefault) {
                var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == partInstance.StockTypeId);
                if (stockType != null) {
                    stockType.Quantity += (int)partInstance.BubblerParameter.Weight;
                    var updated = await this._categoryRepository.UpdateAsync(stockType);
                    if (updated == null) {
                        await this._unitOfWork.Undo();
                        return new ReturnItemOutput(null, false, "Error: Could not adjust stock, Please contact administrator");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new ReturnItemOutput(null,false,"Error: Could not adjust stock, Please contact administrator");
                }
            }

            Transaction transaction = new Transaction(partInstance, InventoryAction.RETURNING,
                partInstance.BubblerParameter.Measured, partInstance.BubblerParameter.Weight, location, item.TimeStamp);
            transaction.UnitCost =0;
            transaction.TotalCost = 0;
            transaction.ReferenceTransactionId = refTransaction.Id;
            transaction.SessionId = this._userService.CurrentSessionId.Value;

            var bubbler = await this._bubblerRepository.UpdateAsync(partInstance.BubblerParameter);
            var instance = await this._partInstanceRepository.UpdateAsync(partInstance);
            var trans = await this._transactionRepository.AddAsync(transaction);

            StringBuilder builder = new StringBuilder();
            if (bubbler != null && instance != null && trans != null) {
                var val = await this._unitOfWork.Save();
                builder.AppendFormat("Instance: {0} Weight: {1} LinesUpdated: {2}", instance.Name, bubbler.Weight, val).AppendLine();
                return new ReturnItemOutput(trans, true, builder.ToString());
            } else {
                await this._unitOfWork.Undo();
                builder.AppendFormat("Instance: {0}", partInstance.Name).AppendLine();
                return new ReturnItemOutput(transaction, false, builder.ToString());
            }
        }

        private async Task<ReturnItemOutput> ExecuteStandard(PartInstance partInstance, Location location, Transaction refTransaction, ReturnItemInput item) {
            partInstance.UpdateQuantity(item.Quantity);

            partInstance.LocationId = location.Id;
            if (item.ConditionId != 0) {
                var condition = await this._categoryRepository.GetEntityAsync(e => e.Id == item.ConditionId);
                if (condition != null) {
                    partInstance.ConditionId = condition.Id;
                }
            }

            if (!partInstance.StockType.IsDefault) {
                var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == partInstance.StockTypeId);
                if (stockType != null) {
                    stockType.Quantity += partInstance.Quantity;
                    var updated = await this._categoryRepository.UpdateAsync(stockType);
                    if (updated == null) {
                        await this._unitOfWork.Undo();
                        return new ReturnItemOutput(null, false, "Error: Could not adjust stock, Please contact administrator");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new ReturnItemOutput(null, false, "Error: Could not adjust stock, Please contact administrator");
                }
            }

            Transaction transaction = new Transaction(partInstance, InventoryAction.RETURNING,
                0, 0, location, item.TimeStamp);
            transaction.Quantity = item.Quantity;

            if (partInstance.CostReported) {
                transaction.UnitCost = refTransaction.UnitCost;
                transaction.TotalCost = refTransaction.TotalCost;
            } else {
                transaction.UnitCost = 0;
                transaction.TotalCost = 0;
            }

            transaction.ReferenceTransactionId = item.ReferenceTransactionId;
            transaction.SessionId = this._userService.CurrentSessionId.Value;

            var instance = await this._partInstanceRepository.UpdateAsync(partInstance);
            var trans = await this._transactionRepository.AddAsync(transaction);
            StringBuilder builder = new StringBuilder();
            if (instance != null && trans != null) {
                var val = await this._unitOfWork.Save();
                builder.AppendFormat("Instance: {0} Quantity:{1} LinesUpdated: {2}", instance.Name, transaction.Quantity, val).AppendLine();
                return new ReturnItemOutput(trans, true, builder.ToString());
            } else {
                await this._unitOfWork.Undo();
                builder.AppendFormat("Instance: {0}", partInstance.Name).AppendLine();
                return new ReturnItemOutput(transaction, false, builder.ToString());
            }
        }



    }
}
