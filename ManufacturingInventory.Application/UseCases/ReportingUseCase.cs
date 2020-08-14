using DevExpress.Mvvm.Native;
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {

    public class MonthlySummaryUseCase : IMonthlyReportUseCase {
        private ManufacturingContext _context;
        private IEntityProvider<PartInstance> _partInstanceProvider;
        private IUnitOfWork _unitOfWork;

        public MonthlySummaryUseCase(ManufacturingContext context) {
            this._context = context;
            this._partInstanceProvider = new PartInstanceProvider(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<MonthlyReportOutput> Execute(MonthlyReportInput input) {
            var dStart = new DateTime(input.StartDate.Year, input.StartDate.Month, input.StartDate.Day, 0, 0, 0, DateTimeKind.Local);
            var dStop = new DateTime(input.StopDate.Year, input.StopDate.Month, input.StopDate.Day, 23, 59, 59, DateTimeKind.Local);
            IEnumerable<PartInstance> partInstances = new List<PartInstance>();
            switch (input.CollectType) {
                case CollectType.OnlyCostReported:
                    partInstances= await this._partInstanceProvider.GetEntityListAsync(instance => instance.CostReported || (instance.IsBubbler && instance.DateInstalled >= dStart && instance.DateInstalled <= dStop));
                    break;
                case CollectType.AllItems:
                    partInstances = await this._partInstanceProvider.GetEntityListAsync(instance => (!instance.IsBubbler) || (instance.IsBubbler && instance.DateInstalled >= dStart && instance.DateInstalled <= dStop));
                    break;
                case CollectType.OnlyCostNotReported:
                    partInstances = await this._partInstanceProvider.GetEntityListAsync(instance => !instance.CostReported || (instance.IsBubbler && instance.DateInstalled >= dStart && instance.DateInstalled <= dStop));
                    break;
            }
            var monthlyReport = new List<PartSummary>();
            StringBuilder transactionBuffer = new StringBuilder();
            foreach (var partInstance in partInstances) {
                var incomingTransactions = from transaction in partInstance.Transactions
                                           where (transaction.TimeStamp >= dStart && transaction.InventoryAction == InventoryAction.INCOMING)
                                           select transaction;

                var outgoingTransactions = from transaction in partInstance.Transactions
                                           where (transaction.TimeStamp >= dStart && transaction.InventoryAction == InventoryAction.OUTGOING)
                                           select transaction;

                var returningTransactions = from transaction in partInstance.Transactions
                                            where (transaction.TimeStamp >= dStart && transaction.InventoryAction == InventoryAction.RETURNING)
                                            select transaction;

                double incomingQtyTotal, incomingQtyRange, outgoingQtyTotal, outgoingQtyRange, currentQty;
                double incomingCostTotal, incomingCostRange, outgoingCostTotal, outgoingCostRange, currentCost;

                if (partInstance.IsBubbler) {
                    incomingQtyTotal = incomingTransactions.Sum(e => e.PartInstance.BubblerParameter.NetWeight);
                    incomingCostTotal = incomingTransactions.Sum(e => e.TotalCost);

                    incomingQtyRange = incomingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.PartInstance.BubblerParameter.NetWeight);
                    incomingCostRange = incomingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.TotalCost);

                    outgoingQtyTotal = outgoingTransactions.Sum(e => e.PartInstance.BubblerParameter.NetWeight);
                    outgoingCostTotal = outgoingTransactions.Sum(e => e.TotalCost);

                    outgoingQtyRange = outgoingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.PartInstance.BubblerParameter.NetWeight);
                    outgoingCostRange = outgoingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.TotalCost);
                    currentQty = partInstance.BubblerParameter.NetWeight;
                    currentCost = partInstance.TotalCost;
                } else {
                    incomingQtyTotal = incomingTransactions.Sum(e => e.Quantity);
                    incomingCostTotal = incomingTransactions.Sum(e => e.TotalCost);

                    incomingQtyRange = incomingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.Quantity);
                    incomingCostRange = incomingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.TotalCost);

                    outgoingQtyTotal = outgoingTransactions.Sum(e => e.Quantity);
                    outgoingCostTotal = outgoingTransactions.Sum(e => e.TotalCost);

                    outgoingQtyRange = outgoingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.Quantity);
                    outgoingCostRange = outgoingTransactions.Where(e => e.TimeStamp <= dStop).Sum(e => e.TotalCost);
                    currentQty = partInstance.Quantity;
                    currentCost = partInstance.TotalCost;
                }

                double outgoingCost = outgoingCostRange;
                double outgoingQty = outgoingQtyRange;

                PartSummary partSummary = new PartSummary();
                partSummary.PartName = partInstance.Part.Name;
                partSummary.InstanceName = partInstance.Name;

                partSummary.StartQuantity = (currentQty - incomingQtyTotal) + outgoingQtyTotal;
                partSummary.StartCost = (currentCost - incomingCostTotal) + outgoingCostTotal;

                partSummary.EndQuantity = (partSummary.StartQuantity + incomingQtyRange) - outgoingQtyRange;
                partSummary.EndCost = (partSummary.StartCost + incomingCostRange) - outgoingCostRange;

                partSummary.IncomingCost = incomingCostRange;
                partSummary.IncomingQuantity = incomingQtyRange;

                partSummary.RndOutgoingCost = outgoingCost / 2;
                partSummary.RndOutgoingQuantity = outgoingQty / 2;

                partSummary.ProductionOutgoingCost = outgoingCost / 2;
                partSummary.ProductionOutgoingQuantity = outgoingQty / 2;

                partSummary.TotalOutgoingCost = outgoingCostRange;
                partSummary.TotalOutgoingQuantity = outgoingQtyRange;

                partSummary.CurrentCost = currentCost;
                partSummary.CurrentQuantity = currentQty;
                monthlyReport.Add(partSummary);

            }
            return new MonthlyReportOutput(monthlyReport, true, "Done");
        }

        public async Task Load() {
            await this._partInstanceProvider.LoadAsync();
        }
    }

    public class CurrentInventoryUseCase : ICurrentInventoryUseCase {
        private ManufacturingContext _context;
        private IEntityProvider<PartInstance> _partInstanceProvider;


        public CurrentInventoryUseCase(ManufacturingContext context) {
            this._context = context;
            this._partInstanceProvider = new PartInstanceProvider(context);
        }

        public async Task<CurrentInventoryOutput> Execute(CurrentInventoryInput input) {
            IEnumerable<PartInstance> parts = new List<PartInstance>();
            switch (input.CollectType) {
                case CollectType.OnlyCostReported:
                    parts = await this._partInstanceProvider.GetEntityListAsync(part => part.CostReported);
                    break;
                case CollectType.AllItems:
                    parts = await this._partInstanceProvider.GetEntityListAsync();
                    break;
                case CollectType.OnlyCostNotReported:
                    parts = await this._partInstanceProvider.GetEntityListAsync(part => !part.CostReported);
                    break;
            }
            List<CurrentInventoryItem> items = new List<CurrentInventoryItem>();
            foreach (var part in parts) {
                if (part.IsBubbler) {
                    items.Add(new CurrentInventoryItem() { Id = part.Id, PartCategory = part.Part.Name, Part = part.Name, Quantity = part.BubblerParameter.NetWeight, Cost = part.TotalCost });
                } else {
                    items.Add(new CurrentInventoryItem() { Id = part.Id, PartCategory = part.Part.Name, Part = part.Name, Quantity = part.Quantity, Cost = part.TotalCost });
                }
            }
            return new CurrentInventoryOutput(items, true, "Success");
        }

        public async Task Load() {
            await this._partInstanceProvider.LoadAsync();
        }
    }

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
