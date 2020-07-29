using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using ManufacturingInventory.Domain.DTOs;

namespace ManufacturingInventory.Application.UseCases {



    public class ReportingUseCase : IReportingUseCase {
        private ManufacturingContext _context;
        //private IEntityProvider<Transaction> _transactionProvider;
        //private IEntityProvider<Part> _partProvider;
        private IEntityProvider<PartInstance> _partInstanceProvider;

        public ReportingUseCase(ManufacturingContext context) {
            this._context = context;
            this._partInstanceProvider = new PartInstanceProvider(context);
            //this._partProvider = new PartProvider(context);
            //this._transactionProvider = new TransactionProvider(context);
        }

        public async Task<ReportingOutput> Execute(ReportingInput input) {
            var dStart = new DateTime(input.StartDate.Year, input.StartDate.Month, input.StartDate.Day, 0, 0, 0, DateTimeKind.Local);
            var dStop = new DateTime(input.StopDate.Year, input.StopDate.Month, input.StopDate.Day, 23, 59, 59, DateTimeKind.Local);

            var partInstances = await this._partInstanceProvider.GetEntityListAsync(instance=>instance.CostReported || (instance.IsBubbler && instance.DateInstalled>=dStart && instance.DateInstalled<=dStop));
            var summary = new List<ReportSnapshot>();
            var transactions = new List<TransactionInfo>();

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

                double outgoingCost = (outgoingCostRange);
                double outgoingQty = (outgoingQtyRange);

                ReportSnapshot snapshotRow = new ReportSnapshot();

                snapshotRow.PartName = partInstance.Part.Name;

                snapshotRow.InstanceName = partInstance.Name;

                snapshotRow.StartQuantity = (currentQty - incomingQtyTotal) + outgoingQtyTotal;
                snapshotRow.StartCost = (currentCost - incomingCostTotal) + outgoingCostTotal;
                if (snapshotRow.StartQuantity > 0) {
                    TransactionInfo transaction = new TransactionInfo();
                    transaction.PartInstanceId = partInstance.Id;
                    transaction.PartInstanceName = partInstance.Name;
                    transaction.LocationId = partInstance.CurrentLocation.Id;
                    transaction.LocationName = partInstance.CurrentLocation.Name;
                    transaction.Action = InventoryAction.INCOMING;
                    transaction.Quantity = snapshotRow.StartQuantity;
                    transaction.UnitCost = partInstance.UnitCost;
                    transaction.TotalCost = snapshotRow.StartQuantity * partInstance.UnitCost;
                    transactions.Add(transaction);
                }

                snapshotRow.EndQuantity = (snapshotRow.StartQuantity + incomingQtyRange) - outgoingQtyRange;
                snapshotRow.EndCost = (snapshotRow.StartCost + incomingCostRange) - outgoingCostRange;

                snapshotRow.CurrentCost = currentCost;
                snapshotRow.CurrentQuantity = currentQty;
                
                snapshotRow.RndOutgoingCost = outgoingCost;
                snapshotRow.RnDOutgoingQuantity = outgoingQty;
                snapshotRow.ProductionOutgoingCost = outgoingCost;
                snapshotRow.ProductionOutgoingQuantity = outgoingQty;

                snapshotRow.IncomingCost = incomingCostRange;
                snapshotRow.IncomingQuantity = incomingQtyRange;

                snapshotRow.TotalOutgoingCost = outgoingCostRange;
                snapshotRow.TotalOutgoingQuantity = outgoingQtyRange;
                summary.Add(snapshotRow);

            }
            return new ReportingOutput(summary,transactions, true, "Done");
        }

        public async Task Load() {
            await this._partInstanceProvider.LoadAsync();
        }
    }
}
