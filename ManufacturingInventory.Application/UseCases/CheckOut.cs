using ManufacturingInventory.Application.Boundaries.Checkout;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {

    public class CheckOut : ICheckOutUseCase {
        private ManufacturingContext _context;
        private IRepository<Transaction> _transactionRepository;
        private IEntityProvider<Location> _locationProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IRepository<PartInstance> _partInstanceRepository;
        private IRepository<BubblerParameter> _bubblerRepository;
        private IUserService _userService;
        private IUnitOfWork _unitOfWork;


        public CheckOut(ManufacturingContext context,IUserService userService) {
            this._bubblerRepository = new BubblerParameterRepository(context);
            this._transactionRepository = new TransactionRepository(context);
            this._locationProvider = new LocationProvider(context);
            this._categoryProvider = new CategoryProvider(context);
            this._partInstanceRepository =new PartInstanceRepository(context);
            this._userService = userService;
            this._unitOfWork = new UnitOfWork(context);
            this._context = context;
        }

        public async Task<IEnumerable<Consumer>> GetConsumers() {
            return (await this._locationProvider.GetEntityListAsync()).OfType<Consumer>();
        }

        public async Task<CheckOutOutput> Execute(CheckOutInput input) {
            CheckOutOutput output = new CheckOutOutput();
            foreach(var item in input.Items) {
                var partInstance = await this._partInstanceRepository.GetEntityAsync(e=>e.Id==item.PartInstanceId);
                var location = await this._locationProvider.GetEntityAsync(e => e.Id == item.LocationId);
                //var bubblerParam
                if (partInstance != null && location != null) {
                    if (item.IsBubbler) {
                        var data=await this.ExecuteBubbler(partInstance, location, item);
                        output.OutputList.Add(data);
                    } else {
                        var data=await this.ExecuteStandard(partInstance, location, item);
                        output.OutputList.Add(data);
                    }
                } else {
                    output.OutputList.Add(new CheckOutOutputData(null, false, "PartInstance or Location Not Found"));
                }
            }
            return output;
        }

        private async Task<CheckOutOutputData> ExecuteBubbler(PartInstance partInstance,Location location,CheckOutInputData item) {
            partInstance.UpdateWeight(item.MeasuredWeight);
            //partInstance.BubblerParameter.DateInstalled = item.TimeStamp;
            partInstance.DateInstalled = item.TimeStamp;
            partInstance.DateRemoved = null;
            partInstance.UpdateQuantity(-1);
            partInstance.CostReported = false;

            partInstance.LocationId = location.Id;
            if (item.ConditionId != 0) {
                var condition = await this._categoryProvider.GetEntityAsync(e => e.Id == item.ConditionId);
                if (condition != null) {
                    partInstance.ConditionId = condition.Id;
                }
            }

            Transaction transaction = new Transaction(partInstance, InventoryAction.OUTGOING,
                partInstance.BubblerParameter.Measured, partInstance.BubblerParameter.Weight, location, item.TimeStamp);

            transaction.SessionId = this._userService.CurrentSession.Id;

            var bubbler = await this._bubblerRepository.UpdateAsync(partInstance.BubblerParameter);
            var instance = await this._partInstanceRepository.UpdateAsync(partInstance);
            var trans = await this._transactionRepository.AddAsync(transaction);
            StringBuilder builder = new StringBuilder();
            if (bubbler != null && instance != null && trans != null) {
                var val = await this._unitOfWork.Save();
                builder.AppendFormat("Instance: {0} Weight: {1} LinesUpdated: {2}", instance.Name, bubbler.Weight, val).AppendLine();
                return new CheckOutOutputData(trans, true, builder.ToString());
            } else {
                await this._unitOfWork.Undo();
                builder.AppendFormat("Instance: {0}", partInstance.Name).AppendLine();
                return new CheckOutOutputData(transaction, false, builder.ToString());
            }
        }

        private async Task<CheckOutOutputData> ExecuteStandard(PartInstance partInstance, Location location, CheckOutInputData item) {
            partInstance.UpdateQuantity(0-item.Quantity);

            //partInstance.LocationId = location.Id;
            if (item.ConditionId != 0) {
                var condition = await this._categoryProvider.GetEntityAsync(e => e.Id == item.ConditionId);
                if (condition != null) {
                    partInstance.ConditionId = condition.Id;
                }
            }

            Transaction transaction = new Transaction(partInstance, InventoryAction.OUTGOING,
                0, 0, location, item.TimeStamp);
            transaction.Quantity = item.Quantity;
            transaction.UnitCost = item.UnitCost;
            transaction.TotalCost = item.Quantity * item.UnitCost;

            transaction.SessionId = this._userService.CurrentSession.Id;

            var instance = await this._partInstanceRepository.UpdateAsync(partInstance);
            var trans = await this._transactionRepository.AddAsync(transaction);
            StringBuilder builder = new StringBuilder();
            if (instance != null && trans != null) {
                var val = await this._unitOfWork.Save();
                builder.AppendFormat("Instance: {0} Quantity:{1} LinesUpdated: {2}", instance.Name,transaction.Quantity, val).AppendLine();
                return new CheckOutOutputData(trans, true, builder.ToString());
            } else {
                await this._unitOfWork.Undo();
                builder.AppendFormat("Instance: {0}", partInstance.Name).AppendLine();
                return new CheckOutOutputData(transaction, false, builder.ToString());
            }
        }

        public async Task<IEnumerable<Condition>> GetConditions() {
            return (await this._categoryProvider.GetEntityListAsync()).OfType<Condition>();
        }
    }
}
