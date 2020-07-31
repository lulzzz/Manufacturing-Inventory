using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Domain.DTOs;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {

    //public class TransactionInfo {
    //    public int PartInstanceId { get; set; }
    //    public string PartInstanceName { get; set; }
    //    public InventoryAction Action { get; set; }
    //    public int LocationId { get; set; }
    //    public string LocationName { get; set; }
    //    public double Quantity { get; set; }
    //    public double UnitCost { get; set; }
    //    public double TotalCost { get; set; }
    //}

    public class MonthlySummaryOutput : IOutput {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<ReportSnapshot> Snapshot { get; set; }

        public MonthlySummaryOutput() {


        }

        public MonthlySummaryOutput(IEnumerable<ReportSnapshot> snapshot,bool success,string message) {
            this.Snapshot = snapshot;
            this.Success = success;
            this.Message = message;
        }
    }

    public class CurrentInventoryOutput : IOutput {

        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<CurrentInventoryItem> CurrentInventoryItems { get; set; }

        public CurrentInventoryOutput() {

        }

        public CurrentInventoryOutput(IEnumerable<CurrentInventoryItem> currentInventoryItems, bool success, string message) {
            this.Success = success;
            this.Message = message;
            this.CurrentInventoryItems = currentInventoryItems;
        }

    }

    public class TransactionSummaryOutput : IOutput {

        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }

        public TransactionSummaryOutput() {

        }

        public TransactionSummaryOutput(IEnumerable<Transaction> transactions,bool success, string message) {
            this.Success = success;
            this.Message = message;
            this.Transactions = transactions;
        }
    }
}
