using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model.Interfaces;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {

    public class MonthlyReportOutput : IOutput {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<PartSummary> MonthlyReport { get; set; }

        public MonthlyReportOutput() {


        }

        public MonthlyReportOutput(IEnumerable<PartSummary> monthlyReport, bool success,string message) {
            this.MonthlyReport = monthlyReport;
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
