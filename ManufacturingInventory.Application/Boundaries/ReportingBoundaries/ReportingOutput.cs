using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model.Interfaces;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {

    public class MonthlySummaryOutput : IOutput {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<IPartMonthlySummary> Snapshot { get; set; }

        public MonthlySummaryOutput() {


        }

        public MonthlySummaryOutput(IEnumerable<IPartMonthlySummary> snapshot,bool success,string message) {
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
