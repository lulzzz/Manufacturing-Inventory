using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {

    public class TransactionLogUseCase : ITransactionLogUseCase {
        private ManufacturingContext _context;
        private IEntityProvider<Transaction> _transactionProvider;

        public TransactionLogUseCase(ManufacturingContext context) {
            this._context = context;
            this._transactionProvider = new TransactionProvider(context);
        }

        public async Task<TransactionSummaryOutput> Execute(TransactionSummaryInput input) {
            var dStart = new DateTime(input.StartDate.Year, input.StartDate.Month, input.StartDate.Day, 0, 0, 0, DateTimeKind.Local);
            var dStop = new DateTime(input.StopDate.Year, input.StopDate.Month, input.StopDate.Day, 23, 59, 59, DateTimeKind.Local);
            IEnumerable<Transaction> transactions = new List<Transaction>();
            switch (input.CollectType) {
                case CollectType.OnlyCostReported:
                    transactions = await this._transactionProvider.GetEntityListAsync(tran => tran.PartInstance.CostReported && (tran.TimeStamp >= dStart && tran.TimeStamp <= dStop));
                    break;
                case CollectType.AllItems:
                    transactions = await this._transactionProvider.GetEntityListAsync(tran => (tran.TimeStamp >= dStart && tran.TimeStamp <= dStop));
                    break;
                case CollectType.OnlyCostNotReported:
                    transactions = await this._transactionProvider.GetEntityListAsync(tran => !tran.PartInstance.CostReported && (tran.TimeStamp >= dStart && tran.TimeStamp <= dStop));
                    break;
            }
            return new TransactionSummaryOutput(transactions, true, "Success");
        }

        public async Task Load() {
            await this._transactionProvider.LoadAsync();
        }
    }
}
