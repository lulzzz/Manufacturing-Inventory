using ManufacturingInventory.Application.Boundaries.Checkout;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {

    public class CheckOutBubbler : ICheckOutBubblerUseCase {
        private ManufacturingContext _context;
        private IRepository<Transaction> _transactionRepository;
        private IEntityProvider<Location> _locationProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IRepository<PartInstance> _partInstanceRepository;
        private IRepository<BubblerParameter> _bubblerRepository;
        private IUserService _userService;
        private IUnitOfWork _unitOfWork;


        public CheckOutBubbler(ManufacturingContext context,IUserService userService) {
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

        public async Task<CheckOutOutput> Execute(CheckOutBubblerInput input) {
            CheckOutOutput output = new CheckOutOutput();
            foreach(var item in input.Items) {
                var partInstance = await this._partInstanceRepository.GetEntityAsync(e=>e.Id==item.PartInstanceId);
                var location = await this._locationProvider.GetEntityAsync(e => e.Id == item.LocationId);
                //var bubblerParam
                if (partInstance != null && location != null) {
                    partInstance.UpdateWeight(item.MeasuredWeight);
                    partInstance.BubblerParameter.DateInstalled = item.TimeStamp;
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
                        partInstance.BubblerParameter.Measured, partInstance.BubblerParameter.Weight,location,item.TimeStamp);

                    transaction.SessionId = this._userService.CurrentSession.Id;

                    var bubbler=await this._bubblerRepository.UpdateAsync(partInstance.BubblerParameter);
                    var instance=await this._partInstanceRepository.UpdateAsync(partInstance);
                    var trans=await this._transactionRepository.AddAsync(transaction);
                    StringBuilder builder = new StringBuilder();
                    if (bubbler!=null && instance!=null && trans != null) {
                        var val = await this._unitOfWork.Save();
                        builder.AppendFormat("Instance: {0} Weight: {1} LinesUpdated: {2}", instance.Name, bubbler.Weight,val).AppendLine();
                        output.OutputList.Add(new CheckOutOutputData(trans, true, builder.ToString()));
                    } else {
                        await this._unitOfWork.Undo();
                        builder.AppendFormat("Instance: {0}", partInstance.Name).AppendLine();
                        output.OutputList.Add(new CheckOutOutputData(transaction, false,builder.ToString()));
                    }
                } else {
                    output.OutputList.Add(new CheckOutOutputData(null, false, "PartInstance or Location Not Found"));
                }
            }
            return output;
        }

        public async Task<IEnumerable<Condition>> GetConditions() {
            return (await this._categoryProvider.GetEntityListAsync()).OfType<Condition>();
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
