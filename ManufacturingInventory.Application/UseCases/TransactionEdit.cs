using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Application.Boundaries.TransactionEdit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class TransactionEdit : ITransactionEditUseCase {
        private IRepository<Transaction> _repository;
        private IUnitOfWork _unitOfWork;
        private ManufacturingContext _context;

        public TransactionEdit(ManufacturingContext context) {
            this._context = context; 
            this._repository = new TransactionRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<TransactionEditOutput> Execute(TransactionEditInput input) {
            return null;
        }
        
        public async Task<IEnumerable<Transaction>> GetTransactions(GetBy by, int id) {
            switch (by) {
                case GetBy.PART:
                    return await this._repository.GetEntityListAsync(e => e.PartInstance.PartId == id);
                case GetBy.PARTINSTANCE:
                    return await this._repository.GetEntityListAsync(e => e.PartInstanceId == id);
                default:
                    return null;
            }
        }

        public async Task Load() {
            await this._repository.LoadAsync();
        }
    }
}
