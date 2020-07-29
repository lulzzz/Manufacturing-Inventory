using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Domain.DTOs;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {

    public class TransactionInfo {
        public int PartInstanceId { get; set; }
        public string PartInstanceName { get; set; }
        public InventoryAction Action { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public double Quantity { get; set; }
        public double UnitCost { get; set; }
        public double TotalCost { get; set; }
    }

    public class ReportingOutput : IOutput {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<ReportSnapshot> Snapshot { get; set; }
        public IEnumerable<TransactionInfo> TransactionsNeeded { get; set; }

        public ReportingOutput() {


        }

        public ReportingOutput(IEnumerable<ReportSnapshot> snapshot,IEnumerable<TransactionInfo> neededTransactions,bool success,string message) {
            this.Snapshot = snapshot;
            this.TransactionsNeeded = neededTransactions;
            this.Success = success;
            this.Message = message;
        }
    }
}
