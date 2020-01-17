using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace ManufacturingInventory.Application.Boundaries.TransactionEdit {
    public class TransactionEditOutput : IOutput {

        public TransactionEditOutput(Transaction transaction, bool success, string message) {
            this.Transaction = transaction;
            this.Success = success;
            this.Message = message;
        }

        public Transaction Transaction { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
