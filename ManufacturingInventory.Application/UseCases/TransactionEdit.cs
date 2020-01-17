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
        public IRepository<Transaction> _repository;
        public IUnitOfWork _unitOfWork;

        public TransactionEdit(IRepository<Transaction> transactionRepositry, IUnitOfWork unitOfWork) {
            this._repository = transactionRepositry;
            this._unitOfWork = unitOfWork;
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
    }
}
