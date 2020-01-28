using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.ReturnItem;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Services;

namespace ManufacturingInventory.Application.UseCases {
    public class ReturnItemUseCase : IReturnItemUseCase {

        private ManufacturingContext _context;
        private IRepository<Transaction> _transactionRepository;
        private IEntityProvider<Location> _locationProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IRepository<PartInstance> _partInstanceRepository;
        private IRepository<BubblerParameter> _bubblerRepository;
        private IEntityProvider<Part> _partProvider;
        private IUserService _userService;
        private IUnitOfWork _unitOfWork;

        public ReturnItemUseCase(ManufacturingContext context, IUserService userService) {
            this._bubblerRepository = new BubblerParameterRepository(context);
            this._transactionRepository = new TransactionRepository(context);
            this._locationProvider = new LocationProvider(context);
            this._categoryProvider = new CategoryProvider(context);
            this._partInstanceRepository = new PartInstanceRepository(context);
            this._partProvider = new PartProvider(context);
            this._userService = userService;
            this._unitOfWork = new UnitOfWork(context);
            this._context = context;
        }

        public async Task<IEnumerable<Condition>> GetConditions() {
            return (await this._categoryProvider.GetEntityListAsync()).OfType<Condition>();
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


        public Task<ReturnItemOutput> Execute(ReturnItemInput input) => throw new NotImplementedException();

    }
}
