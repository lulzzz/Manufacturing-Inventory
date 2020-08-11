using DevExpress.Mvvm.Native;
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using ManufacturingInventory.Domain.DTOs;
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

    public class MonthlySummaryUseCase : IMonthlySummaryUseCase {
        private ManufacturingContext _context;
        private IEntityProvider<PartInstance> _partInstanceProvider;
        private IRepository<MonthlySummary> _monthlySummaryRepo;
        private IUnitOfWork _unitOfWork;

        public MonthlySummaryUseCase(ManufacturingContext context) {
            this._context = context;
            this._partInstanceProvider = new PartInstanceProvider(context);
            this._monthlySummaryRepo = new MonthlySummaryRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<MonthlySummaryOutput> Execute(MonthlySummaryInput input) {
            var dStart = new DateTime(input.StartDate.Year, input.StartDate.Month, input.StartDate.Day, 0, 0, 0, DateTimeKind.Local);
            var dStop = new DateTime(input.StopDate.Year, input.StopDate.Month, input.StopDate.Day, 23, 59, 59, DateTimeKind.Local);
            var partInstances = await this._partInstanceProvider.GetEntityListAsync(instance => instance.CostReported || (instance.IsBubbler && instance.DateInstalled >= dStart && instance.DateInstalled <= dStop));
            var monthlySummary = new MonthlySummary(dStart,dStop);
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

                PartMonthlySummary snapshotRow = new PartMonthlySummary();
                snapshotRow.PartName = partInstance.Part.Name;
                snapshotRow.InstanceName = partInstance.Name;

                snapshotRow.StartQuantity = (currentQty - incomingQtyTotal) + outgoingQtyTotal;
                snapshotRow.StartCost = (currentCost - incomingCostTotal) + outgoingCostTotal;

                snapshotRow.EndQuantity = (snapshotRow.StartQuantity + incomingQtyRange) - outgoingQtyRange;
                snapshotRow.EndCost = (snapshotRow.StartCost + incomingCostRange) - outgoingCostRange;

                snapshotRow.IncomingCost = incomingCostRange;
                snapshotRow.IncomingQuantity = incomingQtyRange;

                snapshotRow.RndOutgoingCost = outgoingCost / 2;
                snapshotRow.RndOutgoingQuantity = outgoingQty / 2;

                snapshotRow.ProductionOutgoingCost = outgoingCost / 2;
                snapshotRow.ProductionOutgoingQuantity = outgoingQty / 2;

                snapshotRow.TotalOutgoingCost = outgoingCostRange;
                snapshotRow.TotalOutgoingQuantity = outgoingQtyRange;

                snapshotRow.CurrentCost = currentCost;
                snapshotRow.CurrentQuantity = currentQty;

                monthlySummary.MonthlyPartSnapshots.Add(snapshotRow);
            }
            return new MonthlySummaryOutput(monthlySummary, true, "Done");
        }

        public async Task<IEnumerable<string>> GetExistingReports() {
            return (await this._monthlySummaryRepo.GetEntityListAsync()).Select(e => e.MonthOfReport);
        }

        public async Task<MonthlySummary> LoadExisitingReport(string month) {
            return await this._monthlySummaryRepo.GetEntityAsync(e => e.MonthOfReport == month);
        }

        public async Task<MonthlySummary> SaveMonthlySummary(MonthlySummary monthlySummary) {
            var entity=await this._monthlySummaryRepo.GetEntityAsync(e => e.Id == monthlySummary.Id);
            if (entity != null) {
                var updated=await this._monthlySummaryRepo.UpdateAsync(monthlySummary);
                if (updated != null) {
                    var count=await this._unitOfWork.Save();
                    if (count > 0) {
                        return updated;
                    } else {
                        await this._unitOfWork.Undo();
                        return null;
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return null;
                }
            } else {
                var newEntity= await this._monthlySummaryRepo.AddAsync(monthlySummary);
                if (newEntity != null) {
                    var count = await this._unitOfWork.Save();
                    if (count > 0) {
                        return newEntity;
                    } else {
                        await this._unitOfWork.Undo();
                        return null;
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return null;
                }
            }
        }

        public async Task Load() {
            await this._partInstanceProvider.LoadAsync();
            await this._monthlySummaryRepo.LoadAsync();
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
            var parts = await this._partInstanceProvider.GetEntityListAsync(part => part.CostReported || input.IncludeAll);
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

    public class TransactionSummaryUseCase : ITransactionSummaryUseCase {
        private ManufacturingContext _context;
        private IEntityProvider<Transaction> _transactionProvider;

        public TransactionSummaryUseCase(ManufacturingContext context) {
            this._context = context;
            this._transactionProvider = new TransactionProvider(context);
        }

        public async Task<TransactionSummaryOutput> Execute(TransactionSummaryInput input) {
            var dStart = new DateTime(input.StartDate.Year, input.StartDate.Month, input.StartDate.Day, 0, 0, 0, DateTimeKind.Local);
            var dStop = new DateTime(input.StopDate.Year, input.StopDate.Month, input.StopDate.Day, 23, 59, 59, DateTimeKind.Local);
             var transactions=await this._transactionProvider.GetEntityListAsync(tran=>(tran.PartInstance.CostReported || input.IncludeAll) && (tran.TimeStamp>=dStart && tran.TimeStamp<=dStop));
            return new TransactionSummaryOutput(transactions, true, "Success");
        }

        public async Task Load() {
            await this._transactionProvider.LoadAsync();
        }
    }
}
