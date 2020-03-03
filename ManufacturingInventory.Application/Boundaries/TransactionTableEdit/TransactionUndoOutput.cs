using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace ManufacturingInventory.Application.Boundaries.TransactionTableEdit {
    public class TransactionUndoOutput : IOutput {

        public TransactionUndoOutput(Transaction transaction, bool success, string message) {
            this.Transaction = transaction;
            this.Success = success;
            this.Message = message;
        }

        public Transaction Transaction { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
