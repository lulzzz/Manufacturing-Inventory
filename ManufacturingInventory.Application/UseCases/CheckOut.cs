using ManufacturingInventory.Application.Boundaries.Checkout;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {

    public class CheckOutBubbler : ICheckOutUseCase {
        private IRepository<Transaction> _transactionRepository;
        private IRepository<Location> _locationRepository;
        private IRepository<Category> _categoryRepository;
        private IRepository<PartInstance> _partInstanceRepository;
        private IUserService _userService;
        private IUnitOfWork _unitOfWork;

        public CheckOutBubbler(
            IUserService userService,
            IRepository<Transaction> transactionRepository,
            IRepository<Location> locationRepository,
            IRepository<Category> categoryRepository,
            IRepository<PartInstance> partInstanceRepository,
            IUnitOfWork unitOfWork) {
            this._userService = userService;
            this._transactionRepository = transactionRepository;
            this._locationRepository = locationRepository;
            this._categoryRepository = categoryRepository;
            this._unitOfWork = unitOfWork;
            this._partInstanceRepository = partInstanceRepository;
        }

        public async Task<IEnumerable<Consumer>> GetConsumers() {
            return (await this._locationRepository.GetEntityListAsync()).OfType<Consumer>();
        }

        public async Task<CheckOutOutput> Execute(CheckOutBubblerInput input) {
            CheckOutOutput output = new CheckOutOutput();
            foreach(var item in input.Items) {
                var partInstance = await this._partInstanceRepository.GetEntityAsync(e=>e.Id==item.PartInstanceId);
                var location = await this._locationRepository.GetEntityAsync(e => e.Id == item.LocationId);
                if (partInstance != null && location != null) {
                    partInstance.UpdateWeight(item.MeasuredWeight);
                    partInstance.UpdateQuantity(-1);
                    partInstance.CostReported = false;                   
                    Transaction transaction = new Transaction(partInstance, InventoryAction.INCOMING, item.TimeStamp, item.Weight, true, location);
                    transaction.SessionId = this._userService.CurrentSession.Id;
                    await this._partInstanceRepository.UpdateAsync(partInstance);
                    await this._transactionRepository.AddAsync(transaction);
                    int val;
                    try {
                        val=await this._unitOfWork.Save();
                    } catch {
                        val = 0;
                    }
                    if (val > 0) {
                        output.OutputList.Add(new CheckOutOutputData(transaction, false, "Success"));

                    } else {
                        output.OutputList.Add(new CheckOutOutputData(null, false, "Failed Checkout"));
                    }
                } else {
                    output.OutputList.Add(new CheckOutOutputData(null, false, "PartInstance or Location Not Found"));
                }
            }
            return output;
        }
    }

    public class CheckOutStandard : IUseCase<CheckOutStandardInput, CheckOutOutput> {
        private IRepository<Transaction> _transactionRepository;
        private IRepository<Location> _locationRepository;
        private IRepository<Category> _categoryRepository;
        private IRepository<PartInstance> _partInstanceRepository;
        private IUserService _userService;
        private IUnitOfWork _unitOfWork;

        public CheckOutStandard(
            IUserService userService,
            IRepository<Transaction> transactionRepository,
            IRepository<Location> locationRepository,
            IRepository<Category> categoryRepository,
            IRepository<PartInstance> partInstanceRepository,
            IUnitOfWork unitOfWork) {
            this._userService = userService;
            this._transactionRepository = transactionRepository;
            this._locationRepository = locationRepository;
            this._categoryRepository = categoryRepository;
            this._unitOfWork = unitOfWork;
            this._partInstanceRepository = partInstanceRepository;
        }

        public async Task<CheckOutOutput> Execute(CheckOutStandardInput items) {
            return null;
        }
    }


    //public class CheckOutMany : IUseCase<CheckOutManyInput> {
    //    private IRepository<Transaction> _transactionRepository;
    //    private IRepository<Location> _locationRepository;
    //    private IRepository<Category> _categoryRepository;
    //    private IRepository<PartInstance> _partInstanceRepository;
    //    private IUnitOfWork _unitOfWork;

    //    public CheckOutMany(
    //        IRepository<Transaction> transactionRepository,
    //        IRepository<Location> locationRepository,
    //        IRepository<Category> categoryRepository,
    //        IRepository<PartInstance> partInstanceRepository,
    //        IUnitOfWork unitOfWork) {
    //        this._transactionRepository = transactionRepository;
    //        this._locationRepository = locationRepository;
    //        this._categoryRepository = categoryRepository;
    //        this._unitOfWork = unitOfWork;
    //        this._partInstanceRepository = partInstanceRepository;
    //    }

    //    public async Task Execute(CheckOutManyInput item) {
    //        foreach(var input in item.InputList) {
    //            var instance = this._partInstanceRepository.GetEntity(e => e.Id == input.PartInstanceId);
    //            var location = this._locationRepository.GetEntity(e => e.Id == input.LocationId);
    //        }
    //    }

    //    private void BuildOperation() {

    //    }

    //    private void BuildOutput() {

    //    }
    //}

    //public class CheckOut {
    //    private IRepository<Transaction> _transactionRepository;
    //    private IRepository<Location> _locationRepository;
    //    private IRepository<Category> _categoryRepository;
    //    private IRepository<PartInstance> _partInstanceRepository;
    //    private IUnitOfWork _unitOfWork;

    //    public CheckOut(
    //        IRepository<Transaction> transactionRepository,
    //        IRepository<Location> locationRepository,
    //        IRepository<Category> categoryRepository,
    //        IRepository<PartInstance> partInstanceRepository,
    //        IUnitOfWork unitOfWork) {
    //        this._transactionRepository = transactionRepository;
    //        this._locationRepository = locationRepository;
    //        this._categoryRepository = categoryRepository;
    //        this._unitOfWork = unitOfWork;
    //        this._partInstanceRepository = partInstanceRepository;
    //    }

    //    public async Task Execute() {

    //    }

    //    private void BuildOperation(IList<Transaction> transactions) {

    //    }

    //}
}
