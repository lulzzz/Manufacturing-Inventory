using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.TransactionEdit {
    public enum GetBy {
        PART,
        PARTINSTANCE
    }

    public interface ITransactionEditUseCase:IUseCase<TransactionEditInput,TransactionEditOutput> {
        Task<IEnumerable<Transaction>> GetTransactions(GetBy by,int id);
    }
}
